using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawScript : MonoBehaviour
{
    public TrailRenderer trail;
    private bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") != 0)
        {
            if(!isActive)
            {
                isActive = true;
                trail.emitting = true;
            }
        }
        else if(isActive)
        {
            isActive = false;
            trail.emitting = false;
        }
    }
}
