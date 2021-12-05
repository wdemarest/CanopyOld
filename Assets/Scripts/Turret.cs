using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Item
{
    [SerializeField] GameObject Rocket;
    [SerializeField] GameObject DieParticles;
    [SerializeField] Transform EyestalkT;
    [SerializeField] GameObject Light;
    float eyeRot = 0;
    [SerializeField] float eyeRotMaxSpeed = 1;
    [SerializeField] float eyeRotSpeed = 0;
    [SerializeField] int pointValue = 6;
    [SerializeField] float verticalBoost = 5;
    Vector3 playerPos;
    Vector3 playerPosAngle;
    Vector3? AimTarget;
    [SerializeField] float fireDelay = 3f;
    [SerializeField] float LockOnDur = 1f;
    [SerializeField] float fireDelayRemaining = 0;
    float range = 25f;
    [SerializeField] bool active = true;

    [SerializeField] AudioSource awaken;
    [SerializeField] AudioSource lockOn;
    [SerializeField] AudioSource fire;

    // Start is called before the first frame update
    void Start()
    {
        fireDelayRemaining = fireDelay;
        points = pointValue;
        
    }

    void rotEyestalk()
    {
        if (active)
        {
            eyeRotSpeed += eyeRotMaxSpeed * 0.3f * Time.deltaTime;
        }
        else
        {
            eyeRotSpeed -= eyeRotMaxSpeed * 0.3f * Time.deltaTime;
        }

        if (eyeRotSpeed > eyeRotMaxSpeed)
        {
            eyeRotSpeed = eyeRotMaxSpeed;
        }
        if (eyeRotSpeed < 0)
        {
            eyeRotSpeed = 0;
        }

        EyestalkT.eulerAngles += new Vector3(0, 0, Time.deltaTime * eyeRotSpeed*360);
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = headObject.GetComponent<Transform>().position;
        playerPosAngle = TargetAngle(playerPos);

        bool playerInSight = !Physics.Raycast(transform.position, playerPosAngle, playerPosAngle.magnitude, LayerMask.GetMask("GrabbableTerrain"));

        if (playerPosAngle.magnitude < range)
        {
            if (!active)
            {
                //Debug.Log(active);
                awaken.Play();
                active = true;
            }
        }
        else
        {
            active = false;
        }

        bool LockedOn = fireDelayRemaining <= LockOnDur;
        if ((active && playerInSight) || LockedOn)
        {
            if (!LockedOn)
            {
                AimTarget = playerPos;
            }

            fireDelayRemaining -= Time.deltaTime;
            GetComponent<Transform>().LookAt(new Vector3(AimTarget.Value.x, AimTarget.Value.y, AimTarget.Value.z));

            if(fireDelayRemaining <= LockOnDur && !LockedOn)
            {
                lockOn.Play();
                LockedOn = true;
            }

            if (fireDelayRemaining <= 0)
            {
                GameObject rocket = Instantiate(Rocket, GetComponent<Transform>().position, Quaternion.Euler(0, 0, 0));
                fire.Play();

                rocket.GetComponent<Rocket>().target = (Vector3)AimTarget;

                fireDelayRemaining = fireDelay;
                AimTarget = null;
            }
        }
        else
        {
            fireDelayRemaining = fireDelay;
            AimTarget = null;
        }

        rotEyestalk();
        Light.SetActive(active);
    }

    Vector3 TargetAngle(Vector3 targetPos)
    {
        Vector3 myPos = GetComponent<Transform>().position;
        
        return targetPos - myPos;
    }

    void Die()
    {
        GameObject explosion = Instantiate(DieParticles, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(3, 3, 3);

        Rigidbody PlayerRB = GameObject.Find("Player").GetComponent<Rigidbody>();
        if(PlayerRB.velocity.y < verticalBoost)
        {
            PlayerRB.velocity = new Vector3(PlayerRB.velocity.x, verticalBoost, PlayerRB.velocity.z);
        }

        OnDeath();
    }

    public override void OnHandCollide()
    {
        Die();
    }
}