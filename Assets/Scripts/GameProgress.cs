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
    static public bool startOnTestLevel = false;
}

public class GameProgress : MonoBehaviour
{
    [SerializeField] GameObject FruitPrefab;
    [SerializeField] GameObject MinePrefab;
    [SerializeField] GameObject TurretPrefab;

    [SerializeField] GameObject cinderBiomeObject;
    [SerializeField] GameObject playerHomeStart;
    [SerializeField] GameObject playerCinderStart;

    public Vase vaseLastTouched;

    private int gameStage = -1;

    [SerializeField] float biomeMoveTime = 5;
    [SerializeField] AnimationCurve biomeMoveSpeed;

    Head head { get { return GameObject.Find("Head").GetComponent<Head>(); } }

    Dictionary<string, GameObject> prefabDict =
        new Dictionary<string, GameObject>();
   
    List<Stage> stageList;

    void Start()
    {
        // WARNING: Setting fog on works for PC, but NOT for Quest 2
        // Here is the fix: https://support.unity.com/hc/en-us/articles/208060696-Enabling-the-Fog-in-a-built-game-is-not-working-
        RenderSettings.fog = true; // Environment.fog;

        stageList = new List<Stage>();
        int t = 6;  // number of trees

        if (DevEnv.startOnTestLevel)
        {
            Debug.Log("Starting on Test Level");
            stageList.Add(new Stage(
                0,
                "MushroomBiome",
                new Dictionary<string, int[]>()
                {
                    ["ForestBiome"] = new int[] { 50 * t, 4 * t, 4 * t }
                },
                rainInterval:300, //120,
                acidCloudInterval:300, //220,
                hunterInterval:300, //190,
                carrierInterval:10, // carrier
                new Color(221, 160, 221, 255)    // purple
            ));
        }
        else
        {
            stageList.Add(new Stage(
                0,
                "",
                null,
                rainInterval: 0,
                acidCloudInterval: 0,
                hunterInterval: 0,
                carrierInterval: 0,
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
            rainInterval: 180,
            acidCloudInterval: 0,
            hunterInterval: 0,
            carrierInterval: 0,
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
            rainInterval: 180,
            acidCloudInterval: 0,
            hunterInterval: 0,
            carrierInterval: 0,
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
            rainInterval: 240,
            acidCloudInterval: 90,
            hunterInterval: 0,
            carrierInterval: 0,
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
            rainInterval: 240,
            acidCloudInterval: 180,
            hunterInterval: 180,
            carrierInterval: 0,
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
            rainInterval: 240,
            acidCloudInterval: 180,
            hunterInterval: 180,
            carrierInterval: 0,
            Color.white
        ));

        prefabDict.Add("Fruit", FruitPrefab);
        prefabDict.Add("Mine", MinePrefab);
        prefabDict.Add("Turret", TurretPrefab);

        head.abyssY = -100;
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
        GameObject.Find("EnvironmentManager").GetComponent<EnvironmentManager>().ClearBiomeTracking();
        GameObject start = cinderBiomeObject!=null ? playerCinderStart : playerHomeStart;
        head.Respawn(start.GetComponent<Transform>().position);
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

    public void AdvanceGameStage(bool instant=false)
    {
        if(gameStage+1 >= stageList.Count)
        {
            Debug.Log("Already at max game stage "+gameStage);
            return;
        }
        gameStage++;
        Debug.Log("Stage=" + gameStage);
        Stage stage = stageList[gameStage];

        if (vaseLastTouched != null && vaseLastTouched.depositedScore != 0)
        {
            vaseLastTouched.Clear();
            vaseLastTouched.SetColor(stage.vaseColor);
        }

        BiomeSetStage(stage);

        if (stage.biome != null)
        {
            stage.biome.PlayAppearSound();
            stage.biome.active = true;
            stage.biome.animations.SetBool("RiseBool", true);
            GameObject.Find("Home").GetComponent<Animator>().SetBool("RiseBool", true);
            if (instant)
            {
                Debug.Log("Instant rez " + stage.biome.name);
                stage.biome.animations.speed = 1000;
            }
            if( gameStage == 1 )
            {
                GameObject.Find("EnvironmentManager").GetComponent<EnvironmentManager>().SetFogEase(12, 0.01f);
            }
        }

    }

    public int GetGameStage()
    {
        return gameStage;
    }

    void Update()
    {
        if( gameStage == -1 )
        {
            AdvanceGameStage(true);
            return;
        }
        Debug.Assert(stageList != null);
        Debug.Assert(gameStage >= 0 && gameStage < stageList.Count);
        if (
            vaseLastTouched != null &&
            !vaseLastTouched.IsAnimatingScore &&
            gameStage < stageList.Count - 1 &&
            vaseLastTouched.depositedScore >= stageList[gameStage + 1].costToAdvance)
        {
            AdvanceGameStage();
        }

//        const float abyssDeathY = 60;
//        if (head.GetComponent<Transform>().position.y > abyssDeathY + 10)
//        {
//            head.abyssY = abyssDeathY;
//        }
    }
}