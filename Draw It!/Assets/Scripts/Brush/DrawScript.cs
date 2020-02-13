using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawScript : MonoBehaviour
{
    public GameObject brushWidthMenu;

    public TrailRenderer[] WhiteTrails = new TrailRenderer[3];
    public TrailRenderer[] RedTrails = new TrailRenderer[3];
    public TrailRenderer[] GreenTrails = new TrailRenderer[3];
    public TrailRenderer[] BlueTrails = new TrailRenderer[3];
    public TrailRenderer[] YellowTrails = new TrailRenderer[3];
    private bool isActive = false;

    private TrailRenderer trail;
    private int currentWidth = 1;
    private string currentColour = "White";

    // Start is called before the first frame update
    void Start()
    {
        trail = WhiteTrails[currentWidth];
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
            if(ChangeTrail(other.name, true))
            {
                currentColour = other.name;
            }
        }
        else if (other.tag == "Width Button")
        {
            switch (other.name)
            {
                case "Small":
                    currentWidth = 0;
                    break;
                case "Medium":
                    currentWidth = 1;
                    break;
                case "Large":
                    currentWidth = 2;
                    break;
            }
            ChangeTrail(currentColour, false);
            brushWidthMenu.GetComponent<BrushWidthMenu>().setButtonSprites(other.GetComponent<SpriteRenderer>());
        }
    }

    private bool ChangeTrail(string _colour, bool changeTip)
    {
        Color newColour;

        switch (_colour)
        {
            case "Red":
                newColour = Color.red;
                trail = RedTrails[currentWidth];
                break;
            case "Green":
                newColour = Color.green;
                trail = GreenTrails[currentWidth];
                break;
            case "Yellow":
                newColour = Color.yellow;
                trail = YellowTrails[currentWidth];
                break;
            case "Blue":
                newColour = Color.blue;
                trail = BlueTrails[currentWidth];
                break;
            case "White":
                newColour = Color.white;
                trail = WhiteTrails[currentWidth];
                break;
            default:
                return false;
        }
        if (changeTip)
        {
            GetComponent<MeshRenderer>().materials[2].color = newColour;
        }
        return true;
    }
}
