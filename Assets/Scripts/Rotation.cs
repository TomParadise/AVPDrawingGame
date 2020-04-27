using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public bool z = false;
    public bool y = false;
    public bool x = false;
    public float speed = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(x)
        {
            this.gameObject.transform.Rotate(speed * Time.deltaTime, 0, 0);
        }

        if (y)
        {
            this.gameObject.transform.Rotate(0, speed * Time.deltaTime, 0);
        }

        if (z)
        {
            this.gameObject.transform.Rotate(0, 0, speed * Time.deltaTime);
        }
    }
}
