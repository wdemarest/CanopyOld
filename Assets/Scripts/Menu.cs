using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    Dictionary<string, bool> controllerPressed = new Dictionary<string, bool>();
    Dictionary<string, bool> latch = new Dictionary<string, bool>();
    bool runOneFrame = false;

    GameProgress gameProgress { get { return GameObject.Find("GameProgress").GetComponent<GameProgress>(); } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ReportControllerState(int handNum, bool primaryButton, bool secondaryButton, bool menuButton)
    {
        void assign(string key, bool newValue)
        {
            if(controllerPressed.ContainsKey(key) ? controllerPressed[key] != newValue : false )
            {
                //Debug.Log("Key " + key + " = " + newValue);
            }
            controllerPressed[key] = newValue;
        }

        if (handNum == 1)
        {
            assign("x", primaryButton);
            assign("y", secondaryButton);
            assign("Escape", menuButton);
        }
        if( handNum == 2 )
        {
            assign("a", primaryButton);
            assign("b", secondaryButton);
        }
    }

    void RunOneFrame()
    {
        runOneFrame = true;
        gameProgress.PauseToggle();
    }

    // Update is called once per frame
    void Update()
    {
        if( runOneFrame )
        {
            gameProgress.PauseToggle();
            runOneFrame = false;
        }
        bool tapped(string key, bool keyPress)
        {
            bool pressed = (controllerPressed.ContainsKey(key) && controllerPressed[key]) || keyPress;
            bool wasLatched = latch.ContainsKey(key) && latch[key];
            latch[key] = pressed;
            if (pressed && !wasLatched)
            {
                return true;
            }
            return false;
        }

        bool xButton = tapped("x",Input.GetKey(KeyCode.X));
        bool yButton = tapped("y", Input.GetKey(KeyCode.Y));
        bool aButton = tapped("a", Input.GetKey(KeyCode.A));
        bool bButton = tapped("b", Input.GetKey(KeyCode.B));
        bool menuButton = tapped("Escape", Input.GetKey(KeyCode.Escape));

        if (menuButton)
        {
            gameProgress.PauseToggle();
        }

        if (gameProgress.IsPaused)
        {
            if (aButton)
            {
                Debug.Log("ManualRespawn");
                gameProgress.RespawnPlayer();
                RunOneFrame();
                return;
            }
            if (xButton)
            {
                gameProgress.RestartGame();
                RunOneFrame();
                return;
            }
            if (yButton)
            {
                gameProgress.AdvanceGameStage();
                RunOneFrame();
                return;
            }
        }

    }
}
