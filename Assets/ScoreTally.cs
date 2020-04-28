using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTally : MonoBehaviour
{
    private TextMeshPro text;
    private GameObject strike;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        strike = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddTally()
    {
        score++;
        if(score > 4)
        {
            strike.SetActive(true);
        }
        else
        {
            text.text += "I";
        }
    }
}
