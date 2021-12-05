using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage
{
    public int costToAdvance;
    public string biomeName;
    public Dictionary<string, int[]> spawn;
    public float rainInterval;
    public float acidCloudInterval;
    public float hunterInterval;
    public float carrierInterval;
    public Color vaseColor;

    public Stage(
        int costToAdvance,
        string biomeName,
        Dictionary<string, int[]> spawn,
        float rainInterval,
        float acidCloudInterval,
        float hunterInterval,
        float carrierInterval,
        Color vaseColor
    )
    {
        Debug.Assert(biomeName == "" || GameObject.Find(biomeName) != null);
        this.costToAdvance = costToAdvance;
        this.biomeName = biomeName;
        this.spawn = spawn;
        this.rainInterval = rainInterval;
        this.acidCloudInterval = acidCloudInterval;
        this.hunterInterval = hunterInterval;
        this.carrierInterval = carrierInterval;
        this.vaseColor = vaseColor;
    }
    public GameObject biomeObject
    {
        get { return biomeName == "" ? null : GameObject.Find(biomeName); }
    }
    public Biome biome
    {
        get { return biomeName == "" ? null : biomeObject.GetComponent<Biome>(); }
    }
}
