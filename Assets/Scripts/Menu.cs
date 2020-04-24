using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class Menu : MonoBehaviour
{
    //Declare a LineRenderer to store the component attached to the GameObject. 
    [SerializeField] LineRenderer rend;

    //Settings for the LineRenderer are stored as a Vector3 array of points. Set up a V3 array to //Initialize in Start. 
    Vector3[] points;

    //Declare a panel to change.
    public GameObject panel;
    public Image one;
    public Image two;
    public Image three;
    public Image four;
    public Image five;
    public GameObject round;
    public GameObject time;
    Image img;
    Button btn;
    bool pause;
    float volume = 0;
    int rounds = 1;
    int timer = 2;
    float i = 0;
    public AudioMixer mixer;

    private bool canPress = false;

    //Start is called before the first frame update
    void Start()
    {
        img = panel.GetComponent<Image>();

        //Get the LineRenderer attached to the GameObject. 
        rend = gameObject.GetComponent<LineRenderer>();

        //Initialize the LineRenderer
        points = new Vector3[2];

        //Set the start point of the LineRenderer to the position of the GameObject. 
        points[0] = 0.035f * transform.forward;

        //Set the end point 20 units away from the GO on the Z axis (pointing forward)
        points[1] = transform.position + transform.forward * 7.5f;

        //Finally set the positions array on the LineRenderer to our new values
        rend.SetPositions(points);
        rend.enabled = true;
    }

    public LayerMask layerMask;

    public bool AlignLineRenderer(LineRenderer rend)
    {
        Ray ray;
        ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        //Debug.DrawRay(ray.origin, ray.direction);
        bool hitBtn = false;

        if (Physics.Raycast(ray, out hit, layerMask))
        {
            points[1] = transform.worldToLocalMatrix.MultiplyVector(transform.forward * hit.distance);
            if (hit.collider.gameObject.tag == "Button")
            {
                rend.startColor = Color.green;
                rend.endColor = Color.green;
                btn = hit.collider.gameObject.GetComponent<Button>();
                hitBtn = true;
            }
            else
            {
                rend.startColor = Color.white;
                rend.endColor = Color.white;
                hitBtn = false;
            }
        }
        else
        {
            points[1] = Vector3.zero;
        }

        rend.SetPositions(points);
        rend.material.color = rend.startColor;
        return hitBtn;
    }

    void Update()
    {
        AlignLineRenderer(rend);
        if (AlignLineRenderer(rend) && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) != 0 && !canPress)
        {
            if (btn.name == "party_btn" || btn.name == "online_btn")
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menu_play");
            }
            else
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Menu/menu_click");
            }
            canPress = true;
            btn.onClick.Invoke();
        }
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) == 0)
        {
            canPress = false;
        }
        if (volume >= -80)
        {
            one.GetComponent<Image>().color = Color.grey;
        }
        if (volume >= -60)
        {
            one.GetComponent<Image>().color = Color.white;
            two.GetComponent<Image>().color = Color.grey;
        }
        if (volume >= -40)
        {
            two.GetComponent<Image>().color = Color.white;
            three.GetComponent<Image>().color = Color.grey;
        }
        if (volume >= -20)
        {
            three.GetComponent<Image>().color = Color.white;
            four.GetComponent<Image>().color = Color.grey;
        }
        if (volume >= 0)
        {
            four.GetComponent<Image>().color = Color.white;
            five.GetComponent<Image>().color = Color.grey;
        }       
        if (volume >= 20)
        {
            five.GetComponent<Image>().color = Color.white;
        }        
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        PlayerPrefs.SetInt("maxTimer", (timer * 20));
        PlayerPrefs.SetInt("maxRounds", rounds);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        SceneManager.LoadScene(sceneName);
    }

    public void VolumeUp()
    {
        if (volume <20)
        {
            i++;
            volume = i * 20;
            mixer.SetFloat("MusicVol", (volume));
        }
    }

    public void VolumeDown()
    {   
        if (volume > -80)
        {
            i--;
            volume = i * 20;
            mixer.SetFloat("MusicVol", (volume));
        }
    }

    public void RoundCUp()
    {
        if (rounds < 5)
        {
            rounds++;

            round.GetComponent<Text>().text = rounds.ToString();
        }
    }

    public void RoundCDown()
    {
        if(rounds > 1)
        {
            rounds--;
            round.GetComponent<Text>().text = rounds.ToString();
        }
    }

    public void RoundTUp()
    {
        if (timer < 3)
        {
            timer++;  
            time.GetComponent<Text>().text = (timer*20).ToString()+"s";
        }
    }

    public void RoundTDown()
    {
        if (timer > 1)
        {
            timer--;
            time.GetComponent<Text>().text = (timer * 20).ToString() + "s";
        }
    }
}
