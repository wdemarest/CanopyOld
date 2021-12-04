using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placer : MonoBehaviour
{
    

    /* public int placed = 0;

    float gasCloudDelay = 10f;
    float gasCloudDelayRemaining = 0;

    float ButterflyDelay = 1f;
    float ButterflyDelayRemaining = 0;

    // Start is called before the first frame update
    void Start()
    {
        Physics.autoSimulation = false;
        for (int i = 0; i < 150; i++)
        {
            GameObject ball = Instantiate(Ball, new Vector3(0, 200 + (i * 35), -100), Quaternion.Euler(0, 0, 0));
            float scale = Random.Range(3, 15);
            ball.transform.localScale = new Vector3(scale, scale, scale);
        }
        

        for (int i = 0; i < 100; i++)
        {
            //Instantiate(Chaser, placePos(150, 175, 50), Quaternion.Euler(90, 0, 0));
        }

        for (int i = 0; i < 500; i++)
        {
            //Instantiate(Mine, placePos(75, 350, 250), Quaternion.Euler(0, 0, 0));
        }
    }

    void Populate()
    {
        for (int i = 0; i < 1000; i++)
        {
            Instantiate(Fruit, placePos(150, 175, 50), Quaternion.Euler(-90, 0, 0));
        }
    }


    // Update is called once per frame
    void Update()
    {
        Physics.autoSimulation = true;

        gasCloudDelayRemaining -= Time.deltaTime;

        if (gasCloudDelayRemaining <= 0)
        {
            gasCloudDelayRemaining = gasCloudDelay;

            //Instantiate(FogCloud, new Vector3(Random.Range(-100, 100), Random.Range(75, 175), -200), Quaternion.Euler(0, 0, 0));
        }

        gasCloudDelayRemaining -= Time.deltaTime;

        if (ButterflyDelayRemaining <= 0)
        {
            ButterflyDelayRemaining = ButterflyDelay;

            Instantiate(Butterfly, new Vector3(Random.Range(-125, 125), Random.Range(75, 175), Random.Range(-125, 125)), Quaternion.Euler(0, 0, 0));
        }

        ButterflyDelayRemaining -= Time.deltaTime;
    }

    Vector3 placePos(float placeWidth, float placeTop, float placeBottom)
    {
        return new Vector3(Random.Range(-1 * placeWidth, placeWidth), Random.Range(placeTop, placeBottom), Random.Range(-1 * placeWidth, placeWidth));
    }

    public bool enoughPlaced()
    {
        return placed >= 500;
    }*/
}
