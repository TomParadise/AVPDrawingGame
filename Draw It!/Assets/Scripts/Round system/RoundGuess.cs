using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundGuess : MonoSingleton<RoundGuess>
{
    public int score = 0;

    //current round, guess right or wrong
    public Dictionary<int, bool> roundScore;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Hand")
        {
            score++;

            RoundManager.Instance.EndRound();
        }
    }
}
