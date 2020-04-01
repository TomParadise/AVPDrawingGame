using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class WordBank : MonoSingleton<WordBank>
{
    private int maxNumbers = 8;
    private List<int> uniqueNumbers;
    private List<int> finishedList;

    [SerializeField] private TextMeshPro[] Text;

    [SerializeField] private TextAsset wordBank;

    private string oldText;
    private List<string> dataLines;
    private string[] words;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void SetText()
    {
        for (int i = 0; i < 2; i++)
        {
            uniqueNumbers = new List<int>();
            finishedList = new List<int>();
            GenerateRandomList();

            int random = Random.Range(0, dataLines.Count);
            words = dataLines[random].Split(',');
            dataLines.Remove(dataLines[random]);

            int j = 0;
            for (int k = 0; k < 8; k++)
            {
                Text[j].text += words[finishedList[k]];

                Text[j].text += "\n";
                j++;
                if (j > 3)
                {
                    j = 0;
                }
            }
        }
    }

    private void GenerateRandomList()
    {
        for (int i = 0; i < maxNumbers; i++)
        {
            uniqueNumbers.Add(i);
        }
        for (int i = 0; i < maxNumbers; i++)
        {
            int ranNum = uniqueNumbers[Random.Range(0, uniqueNumbers.Count)];
            finishedList.Add(ranNum);
            uniqueNumbers.Remove(ranNum);
        }
    }

    public void ResetText()
    {
        for (int i = 0; i < Text.Length; i++)
        {
            Text[i].text = "";
        }
        dataLines = wordBank.text.Split('\n').ToList();

        SetText();
    }
}
