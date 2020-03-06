using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startTimer;
    [SerializeField] private TextMeshPro roundTimer;
    [SerializeField] private GameObject button;
    [SerializeField] private Transform cameraPos;

    private Vector3 buttonSpawnPos;

    private float timer = 3.0f;

    [SerializeField] private float maxTimer = 60.0f;
    private bool startCountdown = false;
    private bool mainCountdown = false;

    private void Start()
    {
        buttonSpawnPos = new Vector3(0.0f, 0.75f, 1.0f);
        if(cameraPos.localPosition.y < 0)
        {
            buttonSpawnPos.y += cameraPos.localPosition.y;
        }
        InsantiateButton();
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
            }
            timer -= Time.deltaTime;
        }
        else
        {
            startTimer.text = Mathf.Ceil(timer).ToString();
            timer -= Time.deltaTime;
        }
    }

    void RoundCountdown()
    {
        timer -= Time.deltaTime;
        roundTimer.text = timer.ToString("F2") + "s";
        if(timer <= 0.0f)
        {
            timer = 0.0f;
            mainCountdown = false;
        }
    }

    void InsantiateButton()
    {
        Instantiate(button, buttonSpawnPos, Quaternion.identity).transform.SetParent(transform);
    }

    public void BeginCountdown()
    {
        startCountdown = true;
    }
}
