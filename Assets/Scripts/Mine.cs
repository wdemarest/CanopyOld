using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Item
{
    [SerializeField] GameObject boomParticles;
    [SerializeField] GameObject dieParticles;
    [SerializeField] GameObject light;
    [SerializeField] float verticalBoost = 5;

    [SerializeField] float fireDelay = 3f;
    float fireDelayRemaining = 0;
    [SerializeField] float damage = 5;
    [SerializeField] float damageRange = 5;
    [SerializeField] float activateRange = 10;
    [SerializeField] float deactivateRange = 15;
    bool active = false;

    [SerializeField] AudioSource awaken;
    [SerializeField] AudioSource sleep;

    // Start is called before the first frame update
    void Start()
    {
        fireDelayRemaining = fireDelay;
    }

    Vector3 TargetAngle(Vector3 targetPos)
    {
        Vector3 myPos = GetComponent<Transform>().position;

        return targetPos - myPos;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = GameObject.Find("Head").GetComponent<Transform>().position;
        Vector3 playerPosAngle = TargetAngle(playerPos);
        float playerDist = playerPosAngle.magnitude;

        bool playerInSight = !Physics.Raycast(transform.position, playerPosAngle, playerDist, LayerMask.GetMask("GrabbableTerrain"));

        if( active && playerDist > deactivateRange )
        {
            sleep.Play();
            active = false;
            light.SetActive(false);
        }
        if( !active && playerDist <= activateRange )
        {
            awaken.Play();
            active = true;
            light.SetActive(true);
        }

        if ( active )
        {
            fireDelayRemaining -= Time.deltaTime;

            if (fireDelayRemaining <= 0)
            {
                GameObject boomObject = Instantiate(boomParticles, transform.position, Quaternion.identity);
                boomObject.GetComponent<Transform>().localScale = new Vector3(damageRange, damageRange, damageRange);
                fireDelayRemaining = fireDelay;

                if (Vector3.Distance(GameObject.Find("Head").GetComponent<Transform>().position, transform.position) < damageRange)
                {
                    GameObject.Find("Head").GetComponent<Head>().takeDamage(damage);
                }
            }
        }
        else
        {
            fireDelayRemaining = fireDelay;
        }
    }

    void Die()
    {
        GameObject explosion = Instantiate(dieParticles, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(3, 3, 3);

        Rigidbody PlayerRB = GameObject.Find("Player").GetComponent<Rigidbody>();
        if (PlayerRB.velocity.y < verticalBoost)
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