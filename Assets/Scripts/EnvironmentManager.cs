using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public GameObject headObject { get { return GameObject.Find("Head"); } }
    public Head head { get { return headObject.GetComponent<Head>(); } }

    Dictionary<Biome, bool> biomeList = new Dictionary<Biome, bool>();
    public bool showFog = true;

    [SerializeField] public Biome defaultBiomeForFog;

    public float overcast = 0;

    Color fogColor = Color.white;
    float fogDensity = 0.01f;

    Color fogTargetColor = Color.white;
    float fogTargetDensity = 0.01f;

    float fogEaseTimer = 0;
    float fogEaseDensity = 0;

    Biome lastValidBiome = null;

    bool firstFrame = true;

    public void SetOvercast(float overcast)
    {
        this.overcast = overcast;
    }

    void Start()
    {
        ClearBiomeTracking();
    }

    public void SetFogEase(float duration, float density)
    {
        fogEaseTimer = duration;
        fogEaseDensity = density;
    }

    void ManageFogEase(ref float fogTargetDensity)
    {
        if (fogEaseTimer > 0)
        {
            fogEaseTimer -= Time.deltaTime;
            fogTargetDensity = fogEaseDensity;
        }
    }

    void FogCalc(Biome biome, float pct, float contribution, ref Color fogColor, ref float fogDensity)
    {
        Color fogAbyss = biome.fogAbyss;
        Color fogTop = biome.fogTop;

        fogColor.r += (fogAbyss.r + (fogTop.r - fogAbyss.r) * pct) * contribution;
        fogColor.g += (fogAbyss.g + (fogTop.g - fogAbyss.g) * pct) * contribution;
        fogColor.b += (fogAbyss.b + (fogTop.b - fogAbyss.b) * pct) * contribution;

        float densityAbyss = biome.densityAbyss;
        float densityTop = biome.densityTop;
        fogDensity += (densityAbyss + (densityTop - densityAbyss) * pct) * contribution;
    }

    void FogLerp(float pct, ref Color fogColor, ref float fogDensity)
    {
        Biome biome = defaultBiomeForFog;
        float dist = 0;
        foreach (Biome b in biomeList.Keys)
        {
            if (!b.active) { continue; }
            if (b.proximity > dist)
            {
                biome = b;
                dist = b.proximity;
            }
        }
        //Debug.Log("ProximateBiome=" + biome.name);
        fogTargetColor.r = 0;
        fogTargetColor.g = 0;
        fogTargetColor.b = 0;
        fogTargetDensity = 0;
        FogCalc(biome, pct, 1, ref fogTargetColor, ref fogTargetDensity);

        ManageFogEase(ref fogTargetDensity);

        void lerpTowards(ref float c, float t, float r)
        {
            c = c < t ? Mathf.Min(c + r, t) : c > t ? Mathf.Max(c - r, t) : c;
        }

        float rate = (1.0f / 3.0f) * Time.deltaTime;
        lerpTowards(ref fogColor.r, fogTargetColor.r, Mathf.Abs((fogTargetColor.r-fogColor.r)*rate));
        lerpTowards(ref fogColor.g, fogTargetColor.g, Mathf.Abs((fogTargetColor.g - fogColor.g) * rate));
        lerpTowards(ref fogColor.b, fogTargetColor.b, Mathf.Abs((fogTargetColor.b - fogColor.b) * rate));
        lerpTowards(ref fogDensity, fogTargetDensity, Mathf.Abs((fogTargetDensity - fogDensity) * rate));
        if( fogColor.r != fogTargetColor.r || fogColor.g != fogTargetColor.g || fogColor.b != fogTargetColor.b || fogDensity != fogTargetDensity )
        {
            //Debug.Log("" + fogColor + "," + fogDensity+" -> "+fogTargetColor+","+fogTargetDensity);
        }
    }

    void FogByContribution(float pct, ref Color fogColor, ref float fogDensity)
    {
        float restore = defaultBiomeForFog.proximity;

        float totalProximity = 0;
        float count = 0;

        foreach (Biome biome in biomeList.Keys)
        {
            if( !biome.active ) { continue; }
            totalProximity += biome.proximity;
            if (biome.proximity > 0)
            {
                count += 1;
            }
        }

        if (count < 2)
        {
            // This is freaky, but what it is saying is that, if we are in "no biome", then
            // for the use of the defaultBiomeForFog's values. AND if we are in only one biome,
            // the defaultBiomeForFog will be the counter-weight.
            defaultBiomeForFog.proximity = defaultBiomeForFog.proximity > 0 ? 1.0f : 1.0f - totalProximity;
            totalProximity = 1.0f;
        }

        fogColor = new Color(0, 0, 0);
        fogDensity = 0;

        foreach (Biome biome in biomeList.Keys)
        {
            if (biome.proximity > 0)
            {
                float contribution = (biome.proximity / totalProximity);
                FogCalc(biome, pct, contribution, ref fogColor, ref fogDensity);
            }
        }

        defaultBiomeForFog.proximity = restore;
    }

    void ManageFog(ref float pct, ref Color fogColor, ref float fogDensity)
    {
        float darknessWhenOvercast = 0.50f;
        pct = pct - (pct * darknessWhenOvercast * overcast);

        GameObject.Find("Sun").GetComponent<Sun>().SetIntensity(1); // pct);

        bool useLerp = true;

        if (useLerp)
        {
            FogLerp(pct, ref fogColor, ref fogDensity);
        }
        else
        {
            FogByContribution(pct, ref fogColor, ref fogDensity);
        }
    }

    void ManageAcidCloud(ref float pct, ref Color fogColor, ref float fogDensity)
    {
        if (head.inCloud)
        {
            fogColor = new Color(
                Mathf.Max(0.8f,fogColor.r),
                Mathf.Min(0.4f,fogColor.g * 0.8f),
                Mathf.Min(0.4f,fogColor.b * 0.8f)
            );
            fogDensity = 0.12f;
            if( !head.acidCloudCrackle.isPlaying )
            {
                head.acidCloudCrackle.Play();
            }
            head.acidCloudCrackle.UnPause();
        }
        else
        {
            head.acidCloudCrackle.Pause();
        }
        head.inCloud = false;
    }

    public void ClearBiomeTracking()
    {
        fogColor = Color.black;
        fogDensity = 1;
        GameObject.Find("MusicManager").GetComponent<MusicManager>().StopAll();
        lastValidBiome = null;
        head.closestValidBiome = null;
    }

    void UpdateBiomeProximity()
    {
        if (head.closestValidBiome != lastValidBiome)
        {
            if (lastValidBiome != null)
            {
                lastValidBiome.OnPlayerExit();
            }
            if (head.closestValidBiome != null)
            {
                head.closestValidBiome.OnPlayerEnter();
                biomeList[head.closestValidBiome] = true;
            }
            lastValidBiome = head.closestValidBiome;
        }
        head.closestValidBiome = null;
    }

    void ManageCinderParticles()
    {
        if(GameObject.Find("CinderBiome") == null )   // An inactive GameObject is excluded from Find() searches
        {
            return;
        }
        Vector3 headPos = headObject.GetComponent<Transform>().position;
        GameObject cinderParticles = GameObject.Find("CinderParticles");
        cinderParticles.GetComponent<Transform>().position = headPos;
        GameObject.Find("CinderSmoke").GetComponent<Transform>().position = new Vector3(headPos.x, GameObject.Find("CinderSmoke").GetComponent<Transform>().position.y, headPos.z);

        if(firstFrame )
        {
            cinderParticles.GetComponent<Transform>().Find("Embers").GetComponent<ParticleSystem>().Play();
            cinderParticles.GetComponent<Transform>().Find("FallingAshes").GetComponent<ParticleSystem>().Play();
            firstFrame = false;
        }
    }

    void Update()
    {
        UpdateBiomeProximity();

        float pct = head.depthPercent;

        ManageFog(ref pct, ref fogColor, ref fogDensity);
        ManageAcidCloud(ref pct, ref fogColor, ref fogDensity);  // must be called after manage fog because it overrides some settings.

        RenderSettings.fog = showFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity; // 0.02f;

        ManageCinderParticles();
    }
}
