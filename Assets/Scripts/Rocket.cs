using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] GameObject DieParticles;
    [SerializeField] TrailRenderer trail;
    GameObject Head;
    public Vector3 target;
    Vector3 startPos;
    Vector3 targetAngle;
    
    Rigidbody RB;
    [SerializeField] float damage = 5;
    [SerializeField] float rocketSpeed = 20;
    [SerializeField] float damageDist = 5;

    // Start is called before the first frame update
    void Start()
    {
        Head = GameObject.Find("Head");
        RB = GetComponent<Rigidbody>();
 
        startPos = GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetAngle = target - startPos;
        if (DistTraveled() >= targetAngle.magnitude)
        {
            Die();
        }


        RB.MovePosition(RB.position + (targetAngle.normalized * Time.deltaTime * rocketSpeed));
    }

    float DistTraveled()
    {
        return (GetComponent<Transform>().position - startPos).magnitude;
    }

    void Die()
    {
        trail.transform.parent = null;

        if (Vector3.Distance(Head.GetComponent<Transform>().position, transform.position) < damageDist)
        {
            Head.GetComponent<Head>().takeDamage(damage);
        }
        GameObject dieParticles = Instantiate(DieParticles, transform.position, Quaternion.identity);
        dieParticles.GetComponent<Transform>().localScale = new Vector3(damageDist, damageDist, damageDist);

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Die();
    }
}
