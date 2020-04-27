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

    [FMODUnity.EventRef]
    FMOD.Studio.EventInstance paintingAudioEvent;
    // Start is called before the first frame update
    void Start()
    {
        //set current colour to white and width to normal
        trail = WhiteTrails[currentWidth];
        particles = new ParticleSystem.Particle[trail.main.maxParticles];
    }

    // Update is called once per frame
    void Update()
    {
        //if pressing the trigger emit 1 particle
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) != 0)
        {
            if (!drawing)
            {
                //Debug.Log("on");
                trail.Emit(1);
                drawing = true;
                paintingAudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Painting/painting");
                paintingAudioEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                paintingAudioEvent.setParameterByName("drawing", 1.0f, true);
                paintingAudioEvent.start();
            }
        }
        //if not pressing the trigger kill particles but keep current trails
        else
        {
            if (drawing)
            {
                StopDrawing();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {      
        //if collided with colour option change particle system and brush tip
        if(other.tag == "Colour")
        {
            if(ChangeTrail(other.name, true))
            {
                currentColour = other.name;
            }
        }
        //if collided with width option change particle system and call width menu to set the button to 'on'
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

    //change trail to input colour and current width
    //change tip colour to input colour if wanted
    private bool ChangeTrail(string _colour, bool changeTip)
    {
        StopDrawing();
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
        if (changeTip && newColour != GetComponent<MeshRenderer>().materials[2].color)
        {
            GetComponent<MeshRenderer>().materials[2].color = newColour;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Colour_change/colour_change", transform.position);
        }
        return true;
    }

    public void StopDrawing()
    {
        if(!trail)
        {
            return;
        }
        if (trail.particleCount > 0)
        {
            trail.GetParticles(particles);
            for (int i = 0; i < trail.particleCount; i++)
            {
                particles[i].remainingLifetime = 0.01f;
            }
            trail.SetParticles(particles);
        }
        if(drawing)
        {
            paintingAudioEvent.setParameterByName("drawing", 0.0f, true);
            paintingAudioEvent.release();
            drawing = false;
        }         
    }

    //loop through all particle systems and set remaining life to 0 so trails die
    public void KillTrails()
    {
        foreach (ParticleSystem particleSystem in WhiteTrails)
        {
            for (int i = 0; i < particleSystem.particleCount; i++)
            {
                particles[i].remainingLifetime = 0;
            }
            particleSystem.SetParticles(particles);
        }
        foreach (ParticleSystem particleSystem in RedTrails)
        {
            for (int i = 0; i < particleSystem.particleCount; i++)
            {
                particles[i].remainingLifetime = 0;
            }
            particleSystem.SetParticles(particles);
        }
        foreach (ParticleSystem particleSystem in GreenTrails)
        {
            for (int i = 0; i < particleSystem.particleCount; i++)
            {
                particles[i].remainingLifetime = 0;
            }
            particleSystem.SetParticles(particles);
        }
        foreach (ParticleSystem particleSystem in BlueTrails)
        {
            for (int i = 0; i < particleSystem.particleCount; i++)
            {
                particles[i].remainingLifetime = 0;
            }
            particleSystem.SetParticles(particles);
        }
        foreach (ParticleSystem particleSystem in YellowTrails)
        {
            for (int i = 0; i < particleSystem.particleCount; i++)
            {
                particles[i].remainingLifetime = 0;
            }
            particleSystem.SetParticles(particles);
        }
    }
}
