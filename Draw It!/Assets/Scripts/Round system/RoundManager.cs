﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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

    private int roundCount = 0;

    private Vector3 buttonSpawnPos;
    private Vector3 wordBankSpawnPos;

    private float timer = 3.0f;

    private bool startCountdown = false;
    private bool mainCountdown = false;

    private bool roundOver = false;

    private void Start()
    {
        //set initial positions and call startRound()
        buttonSpawnPos = new Vector3(1.0f, 0.75f, 0.0f);
        if(cameraPos.localPosition.y < 0)
        {
            buttonSpawnPos.y += cameraPos.localPosition.y;
        }
        wordBankSpawnPos = wordBank.transform.position;
        NewRound();
    }

    // Update is called once per frame
    void Update()
    {
        if(startCountdown)
        {
            StartCountdown();
        }
        else if(mainCountdown)
        {
            RoundCountdown();
        }
    }

    //countdown function for start timer
    void StartCountdown()
    {        
        if(timer <= 0.0f)
        {
            startTimer.text = "DRAW!";
            if(timer <= -1)
            {
                startTimer.text = "";
                startCountdown = false;
                mainCountdown = true;
                timer = maxTimer;
                drawScript.enabled = true;
            }
            timer -= Time.deltaTime;
        }
        else
        {
            startTimer.text = Mathf.Ceil(timer).ToString();
            timer -= Time.deltaTime;
        }
    }

    //countdown function for main round timer
    void RoundCountdown()
    {
        timer -= Time.deltaTime;
        roundTimer.text = timer.ToString("F2") + "s";
        if (timer <= 0.0f)
        {
            mainCountdown = false;
            EndRound();
            roundTimer.text = "";
            return;
        }
    }

    //initialise the start of the round
    public void NewRound()
    {
        roundOver = false;
        drawScript.KillTrails();
        Instantiate(button, button.transform).transform.SetParent(transform);
        timer = 3.0f;

        wordBank.transform.position = wordBankSpawnPos;
        Quaternion rot = wordBank.transform.rotation;
        rot.y = 0;
        wordBank.transform.rotation = rot;

        roundCount++;
    }    

    //begin the initial countdown and move wordbank to behind player
    public void BeginCountdown()
    {
        startCountdown = true;

        wordBank.transform.position = new Vector3(wordBankSpawnPos.x, wordBankSpawnPos.y, -10);
        Quaternion rot = wordBank.transform.rotation;
        rot.y = 180;
        wordBank.transform.rotation = rot;
    }

    //disable drawing and check if game is over
    //called on correct guess and time running out
    public void EndRound()
    {
        drawScript.StopDrawing();
        drawScript.enabled = false;
        if (roundCount == maxRounds)
        {
            startTimer.text = "Game Over";
            Destroy(gameObject);
        }
        else
        {
            RoundOver();
        }
    }

    private void RoundOver()
    {
        Instantiate(button, button.transform).transform.SetParent(transform);
        roundOver = true;
    }

    public bool GetRoundOver()
    {
        return roundOver;
    }
}
