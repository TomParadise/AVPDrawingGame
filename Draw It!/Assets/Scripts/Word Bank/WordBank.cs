using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordBank : MonoBehaviour
{
    public TextMeshPro Text;

    public TextAsset wordBank;

    private string oldText;
    private string[] dataLines;
    public float speed = 5;

    private float timer = 1;
    // Start is called before the first frame update
    void Start()
    {
        dataLines = wordBank.text.Split('\n');



        for (int x = 1; x < 60; x++)
        {

            timer = 1.0f;
            int i = Random.Range(0, 850);


            Text.text = dataLines[i] + oldText;

            if (Text != null)
            {
               if(x%2 == 0)
                {
                    oldText = "\n" + Text.text;
                }

               else
                {
                    oldText = "\t" + "\t" + "\t" + Text.text;
                }
            }



            //Text.text = dataLines[i];
            //Debug.Log(dataLines[i]);
            //Text.text = "Enter Your Text Here";
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f)
        {

        }


        this.gameObject.transform.Translate(0, speed * -Time.deltaTime, 0);
    }
}
