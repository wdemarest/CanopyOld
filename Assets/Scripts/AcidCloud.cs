using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidCloud : Item
{
    float speed = 5.0f;
    float diameterMin = 30.0f;
    float diameterRange = 30.0f;
    float diameterGoal;
    float diameter;
    float timer = 0;
    float easeTime = 10.0f;
    public float zDestroyOffset = 100.0f;

    float damageTimer = 0;
    float damageInterval = 1;
    [SerializeField] AudioSource fogSoundLoop;

    // Start is called before the first frame update
    void Start()
    {
        diameterGoal = Random.Range(diameterMin, diameterMin+ diameterRange);
        diameter = 0;
        GetComponent<Transform>().localScale = new Vector3(diameter, diameter, diameter);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if ( timer < easeTime )
        {
            diameter = Mathf.Min(diameterGoal, diameter + (1 / easeTime) * Time.deltaTime * diameterGoal);
            GetComponent<Transform>().localScale = new Vector3(diameter, diameter, diameter);
        }

        if (transform.position.z > headObject.GetComponent<Transform>().position.z + zDestroyOffset)
        {
            diameter = Mathf.Max(0.0f, diameter - (1 / easeTime) * Time.deltaTime * diameterGoal);
            GetComponent<Transform>().localScale = new Vector3(diameter, diameter, diameter);
            if (diameter <= 0.0f)
            {
                Destroy(gameObject);
                return;
            }
        }

        transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //transform.Rotate(0, 30 * Time.deltaTime, 0);

        bool touchingPlayer = Vector3.Distance(headObject.GetComponent<Transform>().position, GetComponent<Transform>().position) < diameter / 2;
        if (touchingPlayer)
        {
            if (damageTimer >= damageInterval)
            {
                head.takeDamage(1);
                damageTimer -= damageInterval;
            }
            damageTimer += Time.deltaTime;
            head.inCloud = true;
            fogSoundLoop.spatialBlend = 0;
        }
        else
        {
            damageTimer = 0;
            fogSoundLoop.spatialBlend = 1;
        }
    }
}
