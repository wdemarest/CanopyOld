using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
{
    float speed = 2;
    float lifetime = 240;
    float age = 0;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-359, 359), Random.Range(-359, 359), Random.Range(-359, 359));
        transform.Rotate(randomDirection);
        transform.Translate(Vector3.forward * -200);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        if (age > lifetime)
        {
            Destroy(gameObject);
        }

        age += Time.deltaTime;
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
        }
    }
}
