using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : Item
{
    [SerializeField] GameObject CollectionParticles;
    [SerializeField] Material matVal0;
    [SerializeField] Material matVal1;
    [SerializeField] Material matVal2;

    [SerializeField] AudioClip collected;
    bool init = false;

    // Start is called before the first frame update
    void Start()
    {
        allowCollectWhenBasketFull = false;
        //Debug.Log("FruitInstantiated");
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            Init();
        }
    }

    void Init()
    {
        init = true;
        foreach (Transform child in transform)
        {
            if (points == 2)
                child.gameObject.GetComponent<Renderer>().material = matVal1;

            if (points >= 3)
                child.gameObject.GetComponent<Renderer>().material = matVal2;
        }
    }

    public override void OnHandCollide()
    {
        AudioSource.PlayClipAtPoint(collected, transform.position);

        Instantiate(CollectionParticles, transform.position, Quaternion.Euler(0, 0, 0));
        //Debug.Log("touch");
        OnDeath();
    }

    /*void OnCollisionStay(Collision collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (positioning == true)
            {
                transform.localScale = new Vector3(3, 3, 3);
                positioning = false;
                GetComponent<LODGroup>().ForceLOD(-1);
                //Debug.Log("Landed");
            }
        }
    }*/
}
