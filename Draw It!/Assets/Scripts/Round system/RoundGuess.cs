using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundGuess : MonoSingleton<RoundGuess>
{

    public Material correct;
    public Material wrong;
    public Material none;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Hand")
        {
            ChangeImage.Instance.images[RoundManager.Instance.GetCurrentRound() - 1].transform.GetChild(0).GetComponent<Image>().material = correct;
            RoundManager.Instance.EndRound();
        }
    }
}
