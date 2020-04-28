using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundGuess : MonoBehaviour
{
    [SerializeField] private ScoreTally score;
    Vector3 spawnPos;
    private bool down;
    private float startTime;

    private void Start()
    {
        spawnPos = transform.position;
        startTime = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Hand")
        {
            if (RoundManager.Instance.GetInRound())
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Game/score_point", transform.position);
                score.AddTally();
                RoundManager.Instance.EndRound();
            }
        }
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        if (!down)
        {
            pos = Vector3.Slerp(pos, spawnPos + new Vector3(0, 0.15f, 0), (Time.time - startTime)/50);
            if(Vector3.Distance(pos, spawnPos + new Vector3(0, 0.15f, 0)) < 0.01f)
            {
                down = true;
                startTime = Time.time;
            }
        }
        else
        {
            pos = Vector3.Slerp(pos, spawnPos + new Vector3(0, -0.15f, 0), (Time.time - startTime) / 50);
            if ((Vector3.Distance(pos, spawnPos + new Vector3(0, -0.15f, 0)) < 0.01f))
            {
                down = false;
                startTime = Time.time;
            }
        }
        transform.position = pos;
    }
}
