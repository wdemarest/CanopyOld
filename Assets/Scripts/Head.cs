using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class Head : MonoBehaviour
{
    [SerializeField] Rigidbody PlayerRB;
    [SerializeField] Hand LeftHand;
    [SerializeField] Hand RightHand;
    public bool gravGrace = false;
    public bool levitation = false;
    [SerializeField] TMP_Text HeldScore;
    public Vector3 lastStablePos;
    public float thrustRemaining;
    public float thrustMax;
    public bool thrustRanOut = false;
    public bool usingThrust;
    public bool inCloud = false;
    public float abyssY = 40f;
    [SerializeField] float abyssDamage = 15f;
    [SerializeField] float health;
    public int heldScore = 0;
    [SerializeField] public int maxHeldScore = 6;
    [SerializeField] float healthMax = 50;
    [SerializeField] TMP_Text Health;

    public Biome closestValidBiome = null;
    public float closestValidBiomeDist2 = 0;
    public float closestValidBiomeRadius = 0;

    public float moveMaxSpeed = 8.5f;
    [SerializeField] PostProcessVolume damageFX;
    float damageFXDurRemaining;
    float damageFXDurSet;
    float abyssRednessRange = 10;
    [SerializeField] Transform horizonTransform;
    [SerializeField] Transform abyssTransform;

    [SerializeField] AudioSource takeDamageSound;
    [SerializeField] AudioSource headBonk;
    [SerializeField] AudioSource velWind;
    [SerializeField] AudioSource playerDeath;
    [SerializeField] public AudioSource acidCloudCrackle;
    float velWindVol;

    SphereCollider SC;

    GameProgress gameProgress { get { return GameObject.Find("GameProgress").GetComponent<GameProgress>(); } }
    public bool inAbyss { get { return transform.position.y < abyssY; } }

    // Start is called before the first frame update
    void Start()
    {
        SC = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerRB.velocity.magnitude > 13) { PlayerRB.velocity *= 13 / PlayerRB.velocity.magnitude; }

        PlayerRB.useGravity = !(gravGrace || levitation);

        if(inAbyss)
        {
            takeDamage(abyssDamage);
            JumpSave();
        }

        //Set vision redness
        damageFX.weight = Mathf.Max(damageFXDurRemaining * 6, -(transform.position.y - (abyssY + abyssRednessRange))) / abyssRednessRange;

        if (damageFXDurRemaining > 0)
        {
            damageFXDurRemaining -= Time.deltaTime;
        }

        Health.text = ""+health;

        if(health <= 0)
        {
            Debug.Log("DeathRespawn");
            gameProgress.RespawnPlayer();
            GameObject.Find("MusicManager").GetComponent<MusicManager>().PlayOverlay(playerDeath);
        }

        HeldScore.text = heldScore + "/" + maxHeldScore;

        if (usingThrust)
        {
            if (thrustRemaining > 0)
            {
                thrustRemaining -= Time.deltaTime;
            }
            else
            {
                thrustRemaining = 0;
                thrustRanOut = true;
            }
        }
        else
        {
            thrustRanOut = false;
            if (thrustRemaining < thrustMax)
            {
                thrustRemaining += Time.deltaTime;
            }
            else
            {
                thrustRemaining = thrustMax;
            }
        }

        usingThrust = false;

        float volMin = 0.05f;
        float volMax = 0.15f;
        velWindVol = volMin + (volMax-volMin) * ((PlayerRB.velocity.magnitude / moveMaxSpeed));

        velWind.volume = velWindVol;
        velWind.pitch = 1 + (velWindVol / 2);

        CenterHorizonToHead();
    }

    public float depthPercent
    {
        get
        {
            float yBottom = 70;
            float yTop = 135;
            float pct = Mathf.Max(0, Mathf.Min(1, (GetComponent<Transform>().position.y - yBottom) / (yTop - yBottom)));
            return pct;
        }
    }

    void CenterHorizonToHead()
    {
        const float terrainHorOffset = -500;
        const float horizonYOffset = -575;
        horizonTransform.position = transform.position + new Vector3(terrainHorOffset, horizonYOffset, terrainHorOffset);
        horizonTransform.position = new Vector3(horizonTransform.position.x, Mathf.Max(horizonTransform.position.y, 90 + horizonYOffset), horizonTransform.position.z);
        abyssTransform.position = new Vector3(transform.position.x + terrainHorOffset, 0, transform.position.z + terrainHorOffset);
    }

    public void GetPoint(int points)
    {
        heldScore += points;
    }

    public void takeDamage(float damageTaken)
    {
        health -= damageTaken;
        damageFXDurSet = 0.1f + (damageTaken * 0.05f);
        damageFXDurRemaining = damageFXDurSet;
        takeDamageSound.Play();
    }

    public void setLastStable(Vector3 stablePos)
    {
        if (transform.position.y < abyssY + abyssRednessRange)
            return;

        lastStablePos = stablePos;
    }

    public void Respawn(Vector3 playerPosition)
    {
        Debug.Log("Respawn at " + playerPosition);
        LeftHand.grabOverridden = true;
        RightHand.grabOverridden = true;
        lastStablePos = playerPosition;
        JumpSave();
        health = healthMax;
        heldScore = 0;
    }


    public void Impulse(Vector3 impulse)
    {
        PlayerRB.velocity += impulse;
    }

    public void HealToFull()
    {
        health = healthMax;
    }

    public void Deposit(Vase vase)
    {
        gameProgress.Deposit(vase, heldScore);
        heldScore = 0;
        HealToFull();
    }

    public void JumpSave()
    {
        PlayerRB.velocity = new Vector3(0, 0, 0);
        PlayerRB.position = lastStablePos;
        gravGrace = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("GrabbableTerrain"))
        {
            headBonk.Play();
        }
    }

}