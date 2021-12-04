using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    /*public bool positioning = true;

    float popTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("BubbleInstantiated");
        transform.position = placePos();

    }

    // Update is called once per frame
    void Update()
    {

        if (popTimer > 0) {
            popTimer -= Time.deltaTime;
            if(popTimer <= 0)
            {
                transform.position = new Vector3(0, -50, 0);
                //Destroy(gameObject);
            }
        }
    }

    Vector3 placePos()
    {
        return new Vector3(Random.Range(-1 * placeWidth, placeWidth)+150, Random.Range(placeTop, placeBottom), Random.Range(-1 * placeWidth, placeWidth));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player") && popTimer == 0)
        {
            //popTimer = 2;
            //Instantiate(CollectionParticles, transform.position+new Vector3(0, 0.45f, 0), Quaternion.Euler(0, 0, 0));
            
        }

    }*/
}
