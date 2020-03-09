using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    Image[] images;
    int score;

    public Material correct;
    public Material wrong;
    public Material none;

    private void Start()
    {
        images = new Image[RoundManager.Instance.GetMaxRoundCount()];

    }

    private void Update()
    {
        if (RoundGuess.Instance.score == 0)
        {
            images[0].GetComponent<Renderer>().material = none;
            images[1].GetComponent<Renderer>().material = none;
            images[2].GetComponent<Renderer>().material = none;
        }

    }

}
