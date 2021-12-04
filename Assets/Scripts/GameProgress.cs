using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public enum GameMode {DEBUG, PLAYTEST, PRODUCTION};

static public class DevEnv
{
    static public GameMode mode = GameMode.PLAYTEST;
    static public bool fog = true;
    static public bool startOnTestLevel = true;
}

public class GameProgress : MonoBehaviour
{
    
    [SerializeField] GameObject FruitPrefab;
    [SerializeField] GameObject MinePrefab;
    [SerializeField] GameObject TurretPrefab;

    [SerializeField] GameObject PlayerStartPosition;

    [SerializeField] int depositedScore = 0;
    private int gameStage = -1;

    [SerializeField] float biomeMoveTime = 5;
    [SerializeField] AnimationCurve biomeMoveSpeed;

    Dictionary<string, GameObject> prefabDict =
        new Dictionary<string, GameObject>();
   

    public class Stage
    {
        public int costToAdvance;
        public string biomeName;
        public Dictionary<string, int[]> spawn;
        public float rainInterval;
        public float acidCloudInterval;
        public float hunterInterval;
        public Color vaseColor;

        public Stage(
            int costToAdvance,
            string biomeName,
            Dictionary<string, int[]> spawn,
            float rainInterval,
            float acidCloudInterval,
            float hunterInterval,
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
            this.vaseColor = vaseColor;
        }
        public GameObject biomeObject
        {
            get { return biomeName=="" ? null : GameObject.Find(biomeName); }
        }
        public Biome biome
        {
            get { return biomeName=="" ? null : biomeObject.GetComponent<Biome>(); }
        }
    }
   
    [SerializeField] public List<Stage> stageList;

    void Start()
    {
        // WARNING: Setting fog on works for PC, but NOT for Quest 2
        RenderSettings.fog = true; // Environment.fog;

        stageList = new List<Stage>();
        int t = 6;  // number of trees

        if (DevEnv.startOnTestLevel)
        {
            Debug.Log("Starting on Test Level");
            stageList.Add(new Stage(
                0,
                "ForestBiome",
                new Dictionary<string, int[]>()
                {
                    ["ForestBiome"] = new int[] { 50 * t, 4 * t, 4 * t }
                },
                30, //120,
                30, //220,
                30, //190,
                new Color(221, 160, 221, 255)    // purple
            ));
        }
        else
        {
            stageList.Add(new Stage(
                0,
                "",
                null,
                0,
                0,
                0,
                Color.green
            ));
        }
        stageList.Add(new Stage(
            1,
            "ForestBiome",
            new Dictionary<string, int[]>()
            {
                ["ForestBiome"] = new int[] { 50 * t, 0, 0 }
            },
            180,
            0,
            0,
            Color.black
        ));
        stageList.Add(new Stage(
            1,
            "CavernBiome",
            new Dictionary<string, int[]>()
            {
                ["ForestBiome"] = new int[] { 50 * t, 10 * t, 0 },
                ["CavernBiome"] = new int[] { 50, 10, 0 }
            },
            180,
            0,
            0,
            Color.gray
        ));
        stageList.Add(new Stage(
            1,
            "MushroomBiome",
            new Dictionary<string, int[]>()
            {
                ["ForestBiome"] = new int[] { 50 * t, 8 * t, 5 * t },
                ["CavernBiome"] = new int[] { 50, 10, 10 },
                ["MushroomBiome"] = new int[] { 50, 10, 30 }
            },
            240,
            90,
            0,
            Color.cyan
        ));
        stageList.Add(new Stage(
            1,
            "FloatingBiome",
            new Dictionary<string, int[]>()
            {
                ["ForestBiome"] = new int[] { 50 * t, 5*t, 5*t },
                ["CavernBiome"] = new int[] { 50, 10, 10 },
                ["MushroomBiome"] = new int[] { 50, 10, 30 },
                ["FloatingBiome"] = new int[] { 50, 10, 4 }
            },
            240,
            180,
            180,
            Color.white
        ));
        stageList.Add(new Stage(
            1,
            "TwistedBiome",
            new Dictionary<string, int[]>()
            {
                ["ForestBiome"] = new int[] { 50 * t, 5 * t, 5 * t },
                ["CavernBiome"] = new int[] { 50, 10, 10 },
                ["MushroomBiome"] = new int[] { 50, 10, 30 },
                ["FloatingBiome"] = new int[] { 50, 10, 4 },
                ["TwistedBiome"] = new int[] { 15, 0, 4 }
            },
            240,
            180,
            180,
            Color.white
        ));

        prefabDict.Add("Fruit", FruitPrefab);
        prefabDict.Add("Mine", MinePrefab);
        prefabDict.Add("Turret", TurretPrefab);

        RespawnPlayer();
    }

    public void PauseToggle()
    {
        Time.timeScale = !IsPaused ? 0.0f : 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    public bool IsPaused {  get { return Time.timeScale == 0.0f; } }

    public void RespawnPlayer()
    {
        GameObject.Find("Head").GetComponent<Head>().Respawn(PlayerStartPosition.GetComponent<Transform>().position);
    }

    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public bool OnItemDeath(Item item)
    {
        if(item.myBiomeName=="" || item.myMarkerIndex == -1)
            return false;
        Biome biome = GameObject.Find(item.myBiomeName).GetComponent<Biome>();
        biome.MarkerClear(item.myMarkerIndex);
        item.myMarkerIndex = -1;
        return true;
    }

    public void BiomeSetStage(Stage stage)
    {
        if (stage.spawn != null)
        {
            Debug.Log("Spawning stage "+stage);

            foreach (KeyValuePair<string, int[]> entry in stage.spawn)
            {
                Biome targetBiome = GameObject.Find(entry.Key).GetComponent<Biome>();
                Debug.Assert(targetBiome != null); 
                targetBiome.Populate(entry.Value, prefabDict);
            }
        }
        GameObject.Find("RainManager").GetComponent<RainManager>().SetIntervalStandard( stage.rainInterval );
        GameObject.Find("AcidCloudManager").GetComponent<AcidCloudManager>().SetIntervalStandard( stage.acidCloudInterval );
        GameObject.Find("HunterManager").GetComponent<HunterManager>().SetIntervalStandard( stage.hunterInterval );
    }

    public void Deposit(Vase vase, int points)
    {
        vase.NotifyOfDeposit(points);
        depositedScore += points;
    }


    public void AdvanceGameStage(bool instant=false)
    {
        if(gameStage+1 >= stageList.Count)
        {
            Debug.Log("Already at max game stage "+gameStage);
            return;
        }
        gameStage++;
        Debug.Log("Stage=" + gameStage);

        depositedScore = 0;
        Vase vase = GameObject.Find("Vase").GetComponent<Vase>();
        vase.Clear();

        Stage stage = stageList[gameStage];
        GameObject vaseObject = GameObject.Find("Vase");
        vaseObject.GetComponent<Vase>().SetColor(stage.vaseColor);
        BiomeSetStage(stage);

        if (stage.biome != null)
        {
            stage.biome.animations.SetBool("RiseBool", true);
            if( instant )
            {
                Debug.Log("Instant rez " + stage.biome.name);
                stage.biome.animations.speed = 1000;
            }
            GameObject.Find("Head").GetComponent<Head>().BiomeAppearSound();
        }
    }


    void Update()
    {
        if( gameStage == -1 )
        {
            AdvanceGameStage(true);
            return;
        }
        Vase vase = GameObject.Find("Vase").GetComponent<Vase>();
        Debug.Assert(vase != null);
        if (!vase.IsAnimatingScore && gameStage < stageList.Count - 1 && depositedScore >= stageList[gameStage + 1].costToAdvance)
        {
            AdvanceGameStage();
        }
    }
}