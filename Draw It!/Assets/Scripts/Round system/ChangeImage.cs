using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeImage : MonoSingleton<ChangeImage>
{
    public Image[] images;
    public Image prefab;
    int score;

    private void Start()
    {
        images = new Image[RoundManager.Instance.GetMaxRoundCount()];

        for (int i = 0; i < images.Length; i++)
        {
            images[i] = Instantiate(prefab, Vector3.zero, Quaternion.identity, this.transform);
            TextMeshProUGUI currtext = images[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            currtext.text = "Round " + (i + 1).ToString();
            images[i].transform.localPosition = new Vector3(-150 + i * 100, 0);
        }
        //if (RoundManager.Instance.GetMaxRoundCount() > 4)
        //{
        //    this.gameObject.transform.position -= new Vector3(gameObject.transform.position.x - 75, 0, 0);
        //}

    }

    private void Update()
    {
        for (int i = 0; i < images.Length; i++)
        {
            if(i == RoundManager.Instance.GetCurrentRound() - 1)
            {
                images[i].GetComponent<Image>().enabled = true;
            }
            else
            {
                images[i].GetComponent<Image>().enabled = false;
            }
        }
        //if (RoundGuess.Instance.score == 0)
        //{
        //    images[0].GetComponent<Renderer>().material = none;
        //    images[1].GetComponent<Renderer>().material = none;
        //    images[2].GetComponent<Renderer>().material = none;
        //}

    }

}
