using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawScript : MonoBehaviour
{
    [SerializeField] private GameObject brushWidthMenu;

    [SerializeField] private ParticleSystem[] WhiteTrails = new ParticleSystem[3];
    [SerializeField] private ParticleSystem[] RedTrails = new ParticleSystem[3];
    [SerializeField] private ParticleSystem[] GreenTrails = new ParticleSystem[3];
    [SerializeField] private ParticleSystem[] BlueTrails = new ParticleSystem[3];
    [SerializeField] private ParticleSystem[] YellowTrails = new ParticleSystem[3];

    [SerializeField] private Transform brushtip;

    private bool drawing = false;

    private ParticleSystem trail;
    ParticleSystem.Particle[] particles;
    private int currentWidth = 1;
    private string currentColour = "White";

    // Start is called before the first frame update
    void Start()
    {
        trail = WhiteTrails[currentWidth];
        particles = new ParticleSystem.Particle[trail.main.maxParticles];
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) != 0)
        {
            if (!drawing)
            {
                //Debug.Log("on");
                trail.Emit(1);
                drawing = true;
            }
        }
        else
        {
            if (drawing)
            {
                trail.GetParticles(particles);
                for (int i = 0; i < trail.particleCount; i++)
                {
                    particles[i].remainingLifetime = 0.01f;
                }
                trail.SetParticles(particles);
                //Debug.Log("off");
                drawing = false;
            }
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
