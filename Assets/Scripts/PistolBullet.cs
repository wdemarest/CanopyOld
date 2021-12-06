using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    [SerializeField] GameObject Explosion;
    [SerializeField] float speed = 50f;
    [SerializeField] float turnSpeed = 5f;
    [SerializeField] float hitRange = 1;
    [SerializeField] float ageMax = 3;
    float age = 0;

    public AudioSource land;
    
    // Update is called once per frame
    void Update()
    {

        age += Time.deltaTime;
        if (age > ageMax)
        {
            Destroy(gameObject);
        }

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Hunter>() != null && other.GetComponent<Hunter>().active)
        {
            Debug.Log("Hit");
            other.GetComponent<Hunter>().TakeDamage();
            Debug.Log("Chaserdamage");
            Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}