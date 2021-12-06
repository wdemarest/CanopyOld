using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Periodic : MonoBehaviour
{
    [SerializeField] public float timer = 0;
    [SerializeField] float intervalStandard = 0;
    [SerializeField] float intervalVariance = 0.50f;
    public float interval;

    public void RandomizeInterval()
    {
        // Note that this happens to work for an interval of zero, which means "off"
        interval = Random.Range(intervalStandard * (1-intervalVariance), intervalStandard * (1+intervalVariance));
    }

    public void SetIntervalStandard(float intervalStandard)
    {
        this.intervalStandard = intervalStandard;
        RandomizeInterval();
    }

    public virtual void PeriodicUpdate()
    {
    }

    void Update()
    {
        if (interval == 0)
        {
            return;
        }

        timer += Time.deltaTime;

        PeriodicUpdate();

        if (timer >= interval)
        {
            OnTimerComplete();
            timer = 0;
            RandomizeInterval();
        }
    }

    public virtual void OnTimerComplete()
    {

    }
}

public class RainManager : Periodic
{
    [SerializeField] GameObject rainPrefab;

    bool thundered = false;
    [SerializeField] float thunderWarningTime = 5;

    [SerializeField] AudioSource thunder;

    public override void PeriodicUpdate()
    {
        if (timer >= interval - thunderWarningTime)
        {
            float overcast = 1 - Mathf.Max(0, Mathf.Min(1, (interval - timer) / thunderWarningTime));
            //Debug.Log(overcast);
            GameObject.Find("EnvironmentManager").GetComponent<EnvironmentManager>().SetOvercast(overcast);
            if (!thundered) {
                thundered = true;
                thunder.Play();
            }
        }
    }

    public override void OnTimerComplete()
    {
        Vector3 position = GameObject.Find("Head").GetComponent<Transform>().position;
        GameObject rainObject = Instantiate(rainPrefab, position, Quaternion.Euler(0, 0, 0));
        Rain rain = rainObject.GetComponent<Rain>();
        rain.rainDuration = Mathf.Min(rain.rainDuration, interval / 2);
        thundered = false;
    }
}
