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
    public Button one;
    public Button two;
    public Button three;
    public Button four;
    public Button five;
    public GameObject round;
    public GameObject time;
    Image img;
    Button btn;
    bool pause;
    float volume = 0;
    float rounds = 0;
    float timer = 0;
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
        ColorBlock onecb = one.colors;
        ColorBlock twocb = two.colors;
        ColorBlock threecb = three.colors;
        ColorBlock fourcb = four.colors;
        ColorBlock fivecb = five.colors;

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
            onecb.normalColor = Color.grey;
            one.colors = onecb;
        }
        if (volume >= -60)
        {
            onecb.normalColor = Color.white;
            one.colors = onecb;
            twocb.normalColor = Color.grey;
            two.colors = twocb;
        }
        if (volume >= -40)
        {
            twocb.normalColor = Color.white;
            two.colors = twocb;
            threecb.normalColor = Color.grey;
            three.colors = threecb;
        }
        if (volume >= -20)
        {
            threecb.normalColor = Color.white;
            three.colors = threecb;
            fourcb.normalColor = Color.grey;
            four.colors = fourcb;
        }
        if (volume >= 0)
        {
            fourcb.normalColor = Color.white;
            four.colors = fourcb;
            fivecb.normalColor = Color.grey;
            five.colors = fivecb;
        }       
        if (volume >= 20)
        {
            fivecb.normalColor = Color.white;
            five.colors = fivecb;
        }
        switch(rounds)
        {
            case 0:
                round.GetComponent<Text>().text = "1";
                break;
            case 1:
                round.GetComponent<Text>().text = "2";
                break;
            case 2:
                round.GetComponent<Text>().text = "3";
                break;
            case 3:
                round.GetComponent<Text>().text = "4";
                break;
            case 4:
                round.GetComponent<Text>().text = "5";
                break;
            default:
                round.GetComponent<Text>().text = "1";
                break;

        }
        switch (timer)
        {
            case 0:
                time.GetComponent<Text>().text = "30s";
                break;
            case 1:
                round.GetComponent<Text>().text = "45s";
                break;
            case 2:
                round.GetComponent<Text>().text = "60s";
                break;
            default:
                round.GetComponent<Text>().text = "30s";
                break;

        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void VolumeUp()
    {
        if (volume <20)
        {
            i++;
            volume = i * 20;
            mixer.SetFloat("MusicVol", (volume));
            PlayerPrefs.SetFloat("MusicVolume", volume);
            Debug.Log(volume);
        }
    }

    public void VolumeDown()
    {   
        if (volume > -80)
        {
            i--;
            volume = i * 20;
            mixer.SetFloat("MusicVol", (volume));
            PlayerPrefs.SetFloat("MusicVolume", volume);
            Debug.Log(volume);
        }
    }

    public void RoundCUp()
    {
        if (rounds < 4)
        {
            rounds++;
            PlayerPrefs.SetFloat("maxRounds", rounds);
        }
        
    }

    public void RoundCDown()
    {
        if(rounds > 0)
        {
            rounds--;
            PlayerPrefs.SetFloat("maxRounds", rounds);
        }
    }

    public void RoundTUp()
    {
        if (rounds < 2)
        {
            rounds++;
            PlayerPrefs.SetFloat("maxTimer", rounds);
        }
    }

    public void RoundTDown()
    {
        if (timer > 0)
        {
            timer--;
            PlayerPrefs.SetFloat("maxTimer", rounds);
        }
    }
}
