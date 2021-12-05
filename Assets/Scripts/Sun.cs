using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    Light sunLight;
    [SerializeField] Light MoonLight;
    [SerializeField] float time = 0;
    [SerializeField] float cycleDur = 10000;
    float twilightDur = 0.1f;
    [SerializeField] float twilight = 0;
    Transform T;
    
    // Start is called before the first frame update
    void Start()
    {
        sunLight = GetComponent<Light>();
        Debug.Assert(sunLight != null);
        T = GetComponent<Transform>();
    }

    float DistToTime(float targetTime)
    {
        return Mathf.Abs(time - targetTime);
    }

    public void SetIntensity(float intensity)
    {
        sunLight.intensity = intensity;
    }

    // Update is called once per frame
    void Update()
    {
        return;
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
            //sunLight.enabled = true;
            MoonLight.enabled = false;
        }
        else
        {
            //sunLight.enabled = false;
            MoonLight.enabled = true;
            twilight = 1;
        }

        //Sun Fade
        float SunIntensity = (DistToTime(0.75f) - 0.20f) * 20;
        if (SunIntensity > 1) { SunIntensity = 1; }
        if (SunIntensity < 0) { SunIntensity = 0; }
        sunLight.intensity = SunIntensity;

        //Set Temp
        sunLight.colorTemperature = ((1 - twilight) * 4570) + 2000;



        //Sun Move
        T.eulerAngles = new Vector3(time * 360, 0, 0);
    }
}
