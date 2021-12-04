using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    GameObject Head;
    [SerializeField] GameObject FireBurst;
    [SerializeField] GameObject LaserParticles;
    [SerializeField] GameObject LaserFireParticles;
    Vector3 target;
    float fireDelay = 3f;
    float fireDelayRemaining = 0;
    float range = 25f;

    float placeWidth = 150;
    float placeTop = 175;
    float placeBottom = 50;

    // Start is called before the first frame update
    void Start()
    {
        Head = GameObject.Find("Head");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetAngle = TargetAngle();
        target = Head.GetComponent<Transform>().position;

        bool playerInSight = !Physics.Raycast(transform.position, targetAngle, targetAngle.magnitude, LayerMask.GetMask("GrabbableTerrain"));

        if (targetAngle.magnitude < range && playerInSight)
        {
            LaserParticles.SetActive(true);
            LaserParticles.transform.LookAt(new Vector3(target.x, target.y-0.1f, target.z));

            float LaserWidth = 0.1f * (2 -(fireDelayRemaining / fireDelay));
            LaserParticles.GetComponent<LineRenderer>().widthMultiplier = LaserWidth;
            LaserParticles.transform.localScale = new Vector3(1, 1, targetAngle.magnitude);

            
            fireDelayRemaining -= Time.deltaTime;
            if(fireDelayRemaining <= fireDelay-0.1f)
            {
                LaserFireParticles.SetActive(false);
            }

            if (fireDelayRemaining <= 0)
            {
                Head.GetComponent<Head>().takeDamage(5);
                    
                fireDelayRemaining = fireDelay;
                LaserFireParticles.SetActive(true);
            }
        }
        else
        {
            fireDelayRemaining = fireDelay;
            LaserParticles.SetActive(false);
        }
    }

    Vector3 TargetAngle()
    {
        Vector3 myPos = GetComponent<Transform>().position;
        Vector3 targetPos = target;
        return targetPos - myPos;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameObject explosion = Instantiate(FireBurst, transform.position, Quaternion.identity);
            explosion.transform.localScale = new Vector3(3, 3, 3);

            Destroy(gameObject);
        }
    }
}
