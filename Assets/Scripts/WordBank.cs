using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordBank : MonoBehaviour
{
    public TextAsset wordBank;
    private string[] dataLines;

    private float timer = 1;
    // Start is called before the first frame update
    void Start()
    {
        dataLines = wordBank.text.Split('\n');
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {
            timer = 1.0f;
            Debug.Log(dataLines[Random.Range(0,999)]);
        }
    }
}
