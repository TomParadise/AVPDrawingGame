﻿using System.Collections;
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
    public Image[] volumeIndicator;
    public Image[] SFXVolumeIndicator;
    public Image[] musicVolumeIndicator;
    public GameObject round;
    public GameObject time;
    Image img;
    Button btn;
    bool pause;
    float volume = 0.6f;
    float SFXVolume = 1.0f;
    float musicVolume = 1.0f;
    int rounds = 3;
    int timer = 2;
    float i = 0;
    public AudioMixer mixer;

    private bool canPress = false;

    FMOD.Studio.Bus Master;
    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus SFX;

    [FMODUnity.EventRef]
    FMOD.Studio.EventInstance music;

    private void OnDestroy()
    {
        music.release();
        music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    //Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            //menu music
            music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/menu");
            music.start();
        }

        //Set FMOD buses
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");

        Master.setVolume(volume);
        SFX.setVolume(SFXVolume);
        Music.setVolume(musicVolume);

        if (panel != null)
        {
            img = panel.GetComponent<Image>();
        }

        //Get the LineRenderer attached to the GameObject. 
        rend = gameObject.GetComponent<LineRenderer>();

        //Initialize the LineRenderer
        points = new Vector3[2];

        if (gameObject.name == "Painting_Brush")
        {
            //Set the start point of the LineRenderer to the position of the GameObject. 
            points[0] = 0.005f * Camera.main.transform.forward;

            //Set the end point 20 units away from the GO on the Z axis (pointing forward)
            points[1] = transform.position + transform.forward * 5.5f;
        }
        else
        {
            //Set the start point of the LineRenderer to the position of the GameObject. 
            points[0] = 0.035f * transform.forward;

            //Set the end point 20 units away from the GO on the Z axis (pointing forward)
            points[1] = transform.position + transform.forward * 7.5f;
        }
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

        if (Physics.Raycast(ray, out hit, 10, layerMask))
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
            points[1] = points[0];
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
            if (btn.name == "lvl1_btn")
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
        PlayerPrefs.SetString("level", sceneName);

        music.release();
        music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        SceneManager.LoadScene(sceneName);
    }

    public void VolumeUp()
    {
        if (volume < 0.9f)
        {
            volume += 0.2f;
            for(int i = 0; i < volumeIndicator.Length; i++)
            {
                if ((i + 1) * 0.2f <= volume+0.1)
                {
                    volumeIndicator[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    volumeIndicator[i].GetComponent<Image>().color = Color.grey;
                }
            }
        }
        Master.setVolume(volume);
        //mixer.SetFloat("MusicVol", (volume));
    }

    public void VolumeDown()
    {
        if (volume > 0.1f)
        {
            volume -= 0.2f;
            for (int i = 0; i < volumeIndicator.Length; i++)
            {
                if ((i + 1) * 0.2f <= volume)
                {
                    volumeIndicator[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    volumeIndicator[i].GetComponent<Image>().color = Color.grey;
                }
            }
        }
        Master.setVolume(volume);
        //mixer.SetFloat("MusicVol", (volume));
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
    public void SFXVolumeUp()
    {
        if (SFXVolume < 0.9f)
        {
            SFXVolume += 0.2f;
            for (int i = 0; i < SFXVolumeIndicator.Length; i++)
            {
                if ((i + 1) * 0.2f <= SFXVolume + 0.1)
                {
                    SFXVolumeIndicator[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    SFXVolumeIndicator[i].GetComponent<Image>().color = Color.grey;
                }
            }
        }
        SFX.setVolume(SFXVolume);
        //mixer.SetFloat("MusicVol", (volume));
    }

    public void SFXVolumeDown()
    {
        if (SFXVolume > 0.1f)
        {
            SFXVolume -= 0.2f;
            for (int i = 0; i < SFXVolumeIndicator.Length; i++)
            {
                if ((i + 1) * 0.2f <= SFXVolume)
                {
                    SFXVolumeIndicator[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    SFXVolumeIndicator[i].GetComponent<Image>().color = Color.grey;
                }
            }
        }
        SFX.setVolume(SFXVolume);
        //mixer.SetFloat("MusicVol", (volume));
    }
    public void MusicVolumeUp()
    {
        if (musicVolume < 0.9f)
        {
            musicVolume += 0.2f;
            for (int i = 0; i < musicVolumeIndicator.Length; i++)
            {
                if ((i + 1) * 0.2f <= musicVolume + 0.1)
                {
                    musicVolumeIndicator[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    musicVolumeIndicator[i].GetComponent<Image>().color = Color.grey;
                }
            }
        }
        music.setVolume(musicVolume);
        //mixer.SetFloat("MusicVol", (volume));
    }

    public void MusicVolumeDown()
    {
        if (musicVolume > 0.1f)
        {
            musicVolume -= 0.2f;
            for (int i = 0; i < musicVolumeIndicator.Length; i++)
            {
                if ((i + 1) * 0.2f <= musicVolume)
                {
                    musicVolumeIndicator[i].GetComponent<Image>().color = Color.white;
                }
                else
                {
                    musicVolumeIndicator[i].GetComponent<Image>().color = Color.grey;
                }
            }
        }
        music.setVolume(musicVolume);
        //mixer.SetFloat("MusicVol", (volume));
    }
}
