using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoundManager : MonoSingleton<RoundManager>
{    
    [SerializeField] private GameObject button; //Button prefab to start the countdown    
    [SerializeField] private DrawScript drawScript;//Paintbrush draw script for enabling/disabling and killing trails

    [SerializeField] private GameObject wordBank;//Wordbank for moving in front of/behind player    
    [SerializeField] private TextMeshProUGUI startTimer; //Inital countdown timer
    [SerializeField] private TextMeshPro roundTimer; //Main round timer

    [SerializeField] private Transform cameraPos;//Camera pos for lowering button spawn pos

    [SerializeField] private int maxRounds = 3;//Number of rounds
    [SerializeField] private float maxTimer = 60.0f;//Amount of time each round

    [SerializeField] private GameObject ExitMenu;//Menu for exiting/restarting the scene

    public Material wrong;
    private int roundCount = 0;

    private Vector3 buttonSpawnPos;

    private float timer = 3.0f;

    private bool startCountdown = false;
    private bool mainCountdown = false;

    private bool roundOver = false;
    public bool inRound = false;

    [FMODUnity.EventRef]
    FMOD.Studio.EventInstance music;

    public int GetMaxRoundCount()
    {
        return maxRounds;
    }

    public int GetCurrentRound()
    {
        return roundCount;
    }

    new private void OnDestroy()
    {
        music.release();
        music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void Start()
    {
        maxRounds = PlayerPrefs.GetInt("maxRounds");
        maxTimer = PlayerPrefs.GetInt("maxTimer");
        if(PlayerPrefs.GetString("level") == "Forest")
        {
            music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/forest");
        }
        else if (PlayerPrefs.GetString("level") == "Desert")
        {
            music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/desert");
        }
        if (maxTimer == 0)

        {

            maxTimer = 40;

        }

        if(maxRounds == 0)

        {

            maxRounds = 3;

        }
        music.start();
        //set initial positions and call startRound()
        buttonSpawnPos = new Vector3(0.0f, 0.75f, 1.0f);
        if (cameraPos.localPosition.y < 0)
        {
            buttonSpawnPos.y += cameraPos.localPosition.y;
        }
        StartRound();
    }

    // Update is called once per frame
    void Update()
    {
        if (startCountdown)
        {
            StartCountdown();
        }
        else if (mainCountdown)
        {
            RoundCountdown();
        }
        else if(!mainCountdown)
        {
            float oldPitch;
            music.getPitch(out oldPitch);
            if (oldPitch > 1)
            {
                oldPitch -= (timer / maxTimer) / (5) * Time.deltaTime;
                if (oldPitch < 1)
                {
                    oldPitch = 1;
                }
                music.setPitch(oldPitch);
            }
        }
    }

    //countdown function for start timer
    void StartCountdown()
    {
        if (timer <= 0.0f)
        {
            startTimer.text = "DRAW!";
            if (timer <= -1)
            {
                startTimer.text = "";
                startCountdown = false;
                mainCountdown = true;
                timer = maxTimer;
                drawScript.enabled = true;
                inRound = true;
            }
            timer -= Time.deltaTime;
        }
        else
        {
            float oldPitch;
            music.getPitch(out oldPitch);
            oldPitch += (timer / maxTimer) / (5) * Time.deltaTime / (maxTimer / 2);
            music.setPitch(oldPitch);

            startTimer.text = Mathf.Ceil(timer).ToString();
            timer -= Time.deltaTime;
        }
    }

    //countdown function for main round timer
    void RoundCountdown()
    {
        timer -= Time.deltaTime;
        roundTimer.text = "timer\n";
        if (timer < 10)
        {
            roundTimer.text += "0" + timer.ToString("F2") + "S";
        }
        else
        {
            roundTimer.text += timer.ToString("F2") + "S";
        }
        if (timer <= 0.0f)
        {
            mainCountdown = false;
            EndRound();
            roundTimer.text = "timer\n00:00S";
            return;
        }
    }

    //initialise the start of the round
    public void StartRound()
    {
        roundTimer.text = "timer\n" + maxTimer.ToString("F2") + "S";
        roundOver = false;
        drawScript.KillTrails();
        Instantiate(button);
        timer = 3.0f;

        wordBank.gameObject.SetActive(true);
        Quaternion rot = wordBank.transform.rotation;
        rot.y = 0;
        wordBank.transform.rotation = rot;

        roundCount++;
        WordBank.Instance.ResetText();
    }

    //begin the initial countdown and hide wordbank
    public void BeginCountdown()
    {
        startCountdown = true;

        wordBank.gameObject.SetActive(false);
        Quaternion rot = wordBank.transform.rotation;
        rot.y = 180;
        wordBank.transform.rotation = rot;
    }

    //disable drawing and check if game is over
    //called on correct guess and time running out
    public void EndRound()
    {
        inRound = false;
        mainCountdown = false;
        drawScript.StopDrawing();
        drawScript.enabled = false;
        RoundOver();
    }

    private void RoundOver()
    {
        Instantiate(button);
        roundOver = true;
    }

    public bool GetRoundOver()
    {
        return roundOver;
    }

    public bool GetInRound()
    {
        return inRound;
    }

    public void EndGame()
    {
        drawScript.KillTrails();
        drawScript.GetComponent<LineRenderer>().enabled = true;
        drawScript.GetComponent<Menu>().enabled = true;
        ExitMenu.SetActive(true);
    }
}