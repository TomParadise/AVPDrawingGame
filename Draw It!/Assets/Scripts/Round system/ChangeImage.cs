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
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(images.Length * 0.55f, 0.5f);
        for (int i = 0; i < images.Length; i++)
        {
            images[i] = Instantiate(prefab, this.transform);
            TextMeshProUGUI currtext = images[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
            currtext.text = "Round " + (i + 1).ToString();
            images[i].transform.localPosition = new Vector3((i*0.55f) + ((images.Length-1)*-0.275f),0,0);
        }
    }

    public void ChangeImages()
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (i == RoundManager.Instance.GetCurrentRound() - 1)
            {
                images[i].GetComponent<Image>().enabled = true;
            }
            else
            {
                images[i].GetComponent<Image>().enabled = false;
            }
        }
    }

}
