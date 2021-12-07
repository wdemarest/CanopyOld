using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour
{
    string name {
        get { return gameObject.name; }
    }
    List<GameObject> markerList;
    [SerializeField] public bool active = false;
    int markerIndex = 0;
    System.Random rng = new System.Random();

    [SerializeField] float ambientFalloff = 20;
    public float ambientPercent = 0;
    public float proximity = 0;

    [SerializeField] public Color fogAbyss = new Color(0.15f,0.15f,0.15f);
    [SerializeField] public float densityAbyss = 0.10f;
    [SerializeField] public Color fogTop = new Color(0.70f, 0.70f, 0.70f);
    [SerializeField] public float densityTop = 0.01f;

    [SerializeField] AudioSource music;
    [SerializeField] AudioSource ambient;
    [SerializeField] AudioSource appearSound;

    public Animator animations;

    void Start()
    {
        markerList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Marker")).FindAll(marker => marker.transform.IsChildOf(this.transform));
        Debug.Assert(markerList != null);
        // WARNING: Do not shuffle more than once, because the items created refer back to the marker order
        Shuffle(markerList);    // so that items are created in random order
    }

    void Update()
    {
        UpdateAmbient();
    }

    void UpdateAmbient()
    {
        if( ambient.isPlaying && ambientPercent <= 0 )
        {
            ambient.Stop();
        }
        ambient.volume = ambientPercent;
        if( !ambient.isPlaying && ambientPercent > 0 )
        {
            ambient.Play();
        }
        if(ambient.isPlaying)
        {
            //Debug.Log("Ambient " + name + "=" + ambientPercent);
        }
        ambientPercent = 0;
    }

    public void Log(string s)
    {
        Debug.Log(s);
    }

    MusicManager musicManager { get { return GameObject.Find("MusicManager").GetComponent<MusicManager>(); } }

    public void PlayAppearSound()
    {
        musicManager.PlayOverlay(appearSound);
    }

    public void OnPlayerEnter()
    {
        Debug.Log("player enter " + name);
        musicManager.PlayMusic(music);
    }

    public void OnPlayerExit()
    {
        Debug.Log("player exit " + name);
        musicManager.FadeMusic();
    }


    public void MarkerClear(int index)
    {
        markerList[index].GetComponent<Marker>().typeMade = "";
    }

    public void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public class EntityTracker
    {
        public String name;
        public int index;
        public int numToMake;
        public int numExist;
        public int numMade;
        public EntityTracker(String name, int index, int numToMake)
        {
            this.name = name;
            this.index = index;
            this.numToMake = numToMake;
            this.numExist = 0;
            this.numMade = 0;
        }
    }

    public void Populate(int[] spawnCounts, Dictionary<string, GameObject> prefabDict)
    {
        if (spawnCounts == null)
        {
            return;
        }
        Debug.Log("Populating");
        int totalMade = 0;

        void entityCreate(Marker markerScript, String entityName, int markerIndex)
        {
            GameObject prefab = prefabDict[entityName];
//            GameObject entity = Instantiate(prefab, gameObject.GetComponent<Transform>());
            GameObject entity = Instantiate(prefab, markerScript.gameObject.GetComponent<Transform>().position, Quaternion.Euler(0, 0, 0));
            entity.transform.parent = gameObject.GetComponent<Transform>();
            Debug.Assert(entity != null);
            Item item = entity.GetComponent<Item>();
            Debug.Assert(item != null);
            Debug.Assert(item.myMarkerIndex == -1);
            item.myBiomeName = this.name;
            item.myMarkerIndex = markerIndex;
            markerScript.typeMade = entityName;
            totalMade++;

            if (entityName == "Fruit")
            {
                if (markerScript.Floating)
                {
                    double randDouble = rng.NextDouble();
                    if (randDouble < 0.5)
                    {
                        item.points = 2;
                    }
                    if (randDouble < 0.1)
                    {
                        item.points = 4;
                    }
                }
                else
                {
                    item.points = 1;
                }
            }
        }

        const int entityLength = 3;
        EntityTracker[] entityResult = new EntityTracker[entityLength];
        List<EntityTracker> entityLookup = new List<EntityTracker>();
        void initEntityLookupForThisLevel()
        {
            Debug.Assert(spawnCounts.Length == entityLength);
            String[] entityIndexToString = new string[] { "Fruit", "Mine", "Turret" };
            for (int i = 0; i < entityLength; ++i)
            {
                String entityName = entityIndexToString[i];
                entityResult[i] = new EntityTracker(entityName, i, spawnCounts[i]);
                Debug.Assert(markerList != null);
                for (int m = 0; m < markerList.Count; ++m)
                {
                    Marker marker = markerList[m].GetComponent<Marker>();
                    if (marker.typeMade == entityName)
                    {
                        entityResult[i].numToMake--;
                        entityResult[i].numExist++;
                    }
                }
                if (entityResult[i].numToMake > 0)
                {
                    entityLookup.Add(entityResult[i]);
                }
            }
        }

        // Populate entityLookup with ONLY the entities that we are making more than zero of.
        initEntityLookupForThisLevel();
        if (entityLookup.Count <= 0)
        {
            return;
        }

        // Run through the marker list only one time
        int entityIndex = 0;
        int markerReps = markerList.Count;
        int[] numMade = new int[3];
        while (entityLookup.Count > 0 && markerReps-- > 0)
        {
            markerIndex = (markerIndex + 1) % markerList.Count;
            Marker markerScript = markerList[markerIndex].GetComponent<Marker>();
            if (markerScript.typeMade != "")
            {
                Debug.Log("skipped #" + markerIndex + " " + markerScript.typeMade);
                continue;
            }
            int reps = entityLookup.Count;
            while (reps > 0)
            {
                entityIndex = (entityIndex + 1) % entityLookup.Count;
                EntityTracker e = entityLookup[entityIndex];
                if (markerScript.flags.ContainsKey(e.name) && markerScript.flags[e.name])
                {
                    entityCreate(markerScript, e.name, markerIndex);
                    e.numMade += 1;
                    e.numToMake -= 1;
                    // If there are no more to make, remove this entity from the list.
                    if (e.numToMake <= 0)
                    {
                        entityLookup.RemoveAt(entityIndex);
                        entityIndex = 0;    // Need to reset here, in case it is higher than the end of the list
                    }
                    break;
                }
                --reps;
            }
        }

        string s = "Biome " + name+" ("+ entityResult.Length+")";
        for (int i = 0; i < entityResult.Length; ++i)
        {
            s += "; "+entityResult[i].name + " made " + entityResult[i].numMade + ", exist " + entityResult[i].numExist + ", failed " + entityResult[i].numToMake;
        }
        Debug.Log(s);
    }

    public void ReportDistance(float dist, float radius)
    {
        float outer = radius;
        float inner = outer - ambientFalloff;
        float pct = dist > outer ? 0.0f : dist < inner ? 1.0f : (outer - dist) / ambientFalloff;
        proximity = pct;
        ambientPercent = Mathf.Max(ambientPercent, pct);
    }
}
