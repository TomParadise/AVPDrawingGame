﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawScript : MonoBehaviour
{
    public TrailRenderer whiteTrail;
    public TrailRenderer RedTrail;
    public TrailRenderer BlueTrail;
    public TrailRenderer YellowTrail;
    public TrailRenderer GreenTrail;
    private bool isActive = false;

    private TrailRenderer trail;

    // Start is called before the first frame update
    void Start()
    {
        trail = whiteTrail;
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

    private void OnTriggerEnter(Collider other)
    {      
        if(other.tag == "Colour")
        {
            Color newColour;
            switch(other.name)
            {
                case "Red":
                    newColour = Color.red;
                    trail = RedTrail;
                    break;
                case "Green":
                    newColour = Color.green;
                    trail = GreenTrail;
                    break;
                case "Yellow":
                    newColour = Color.yellow;
                    trail = YellowTrail;
                    break;
                case "Blue":
                    newColour = Color.blue;
                    trail = BlueTrail;
                    break;
                case "White":
                    newColour = Color.white;
                    trail = whiteTrail;
                    break;
                default:
                    return;
            }
            GetComponent<MeshRenderer>().materials[2].color = newColour;            
        }
    }
}
