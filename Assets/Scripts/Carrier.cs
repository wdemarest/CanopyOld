using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : Item
{
    [SerializeField] GameObject dieParticles;
    [SerializeField] AudioSource soundAppear;
    [SerializeField] AudioSource soundTravel;

    float speed = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
        soundAppear.Play();
        soundTravel.Play();
    }

    public void Init(float distForward, float distRight)
    {
        Transform pt = headObject.GetComponent<Transform>();
        transform.position = pt.position;
        transform.rotation = pt.rotation;
        transform.localScale = pt.localScale;
        transform.position += pt.forward * distForward; // + pt.right * distRight;
        transform.Rotate(0.0f, -90.0f, 0.0f, Space.World);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void Die()
    {
        Instantiate(dieParticles, transform.position, transform.rotation);
        OnDeath();
    }

    public override void OnHandCollide()
    {
        Die();
    }
}
