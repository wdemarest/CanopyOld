using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Item
{
    [SerializeField] GameObject Fins;
    [SerializeField] GameObject Light;
    [SerializeField] GameObject DieParticles;
    [SerializeField] GameObject DetonateParticles;
    [SerializeField] float minSpeed = 2.5f;
    [SerializeField] float maxSpeed = 3.5f;
    float speed;
    [SerializeField] float damage = 5;
    [SerializeField] float turnSpeed = 1;
    [SerializeField] int health = 10;
    [SerializeField] float activateRange = 50;
    [SerializeField] float damageDist = 5;
    public bool active = false;
    [SerializeField] AudioSource takeDamage;

    public GameObject target {  get { return headObject; } }

    // Start is called before the first frame update
    void Start()
    {
        speed = minSpeed + (Random.value * (maxSpeed - minSpeed));
    }

    Chaser Init()
    {
        Vector3 position = headObject.GetComponent<Transform>().position;
        position.x -= 50.0f;
        return this;
    }

    // Update is called once per frame
    void Update()
    {
        Fins.SetActive(active);
        Light.SetActive(active);

        Vector3 deltaToTarget = target.GetComponent<Transform>().position - transform.position;

        if (deltaToTarget.magnitude < activateRange && !active)
        {
            active = true;
        }

        if (active)
        {
            Vector3 targetDirection = Vector3.Normalize(deltaToTarget);
            float angle = Vector3.Angle(targetDirection, transform.forward);

            float turnAmount = turnSpeed * ((angle - 3) / 180) * Time.deltaTime;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, turnAmount, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            transform.Translate(Vector3.forward * Time.deltaTime * speed);

            Fins.GetComponent<Transform>().Rotate(0.0f, 0.0f, -720.0f * Time.deltaTime, Space.Self);

            if (Vector3.Distance(target.transform.position, transform.position) < damageDist)
            {
                target.GetComponent<Head>().takeDamage(damage);
                Detonate();
            }
        }
    }

    public void TakeDamage()
    {
        if (active)
        {
            health -= 1;

            takeDamage.Play();

            if (health <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        Instantiate(DieParticles, transform.position, transform.rotation);
        OnDeath();
    }

    void Detonate()
    {
        Instantiate(DetonateParticles, transform.position, transform.rotation);
        OnDeath();
    }
}
