using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSway : MonoBehaviour
{
    public float maxSway = 15f;
    public float swaySpeed = 1f;
    private float counter;
    private float maxCounter;
    // Start is called before the first frame update
    void Start()
    {
        maxCounter = Random.Range(5, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if(counter < maxCounter)
        {
            counter += Time.deltaTime;
            this.gameObject.transform.Rotate(swaySpeed * Time.deltaTime, 0, 0);
        }

        else if (counter < maxCounter * 2)
        {
            counter += Time.deltaTime;
            this.gameObject.transform.Rotate(-swaySpeed * Time.deltaTime, 0, 0);
        }

        else
        {
            counter = 0;
        }
    }
}
