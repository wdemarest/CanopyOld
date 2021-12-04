using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    Light SunLight;
    [SerializeField] Light MoonLight;
    [SerializeField] float time = 0;
    [SerializeField] float cycleDur = 10000;
    float twilightDur = 0.1f;
    [SerializeField] float twilight = 0;
    Transform T;
    
    // Start is called before the first frame update
    void Start()
    {
        SunLight = GetComponent<Light>();
        T = GetComponent<Transform>();
    }

    float DistToTime(float targetTime)
    {
        return Mathf.Abs(time - targetTime);
    }

    // Update is called once per frame
    void Update()
    {
        //Time Pass
        //time += Time.deltaTime/cycleDur;

        if(time > 1)
        {
            time -= 1;
        }

        //Twilight Calc
        if(time < 0.25f)
        {
            twilight = 1 - (DistToTime(0)*(1 / twilightDur));
        }
        else
        {
            twilight = 1 - (DistToTime(0.5f) * (1 / twilightDur));
        }
        if (twilight > 1) { twilight = 1; }
        if (twilight < 0) { twilight = 0; }

        //Night and Day
        if (time <= 0.5)
        {
            //SunLight.enabled = true;
            MoonLight.enabled = false;
        }
        else
        {
            //SunLight.enabled = false;
            MoonLight.enabled = true;
            twilight = 1;
        }

        //Sun Fade
        float SunIntensity = (DistToTime(0.75f) - 0.20f) * 20;
        if (SunIntensity > 1) { SunIntensity = 1; }
        if (SunIntensity < 0) { SunIntensity = 0; }
        SunLight.intensity = SunIntensity;

        //Set Temp
        SunLight.colorTemperature = ((1 - twilight) * 4570) + 2000;



        //Sun Move
        T.eulerAngles = new Vector3(time * 360, 0, 0);
    }
}
