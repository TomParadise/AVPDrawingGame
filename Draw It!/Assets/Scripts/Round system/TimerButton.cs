using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class TimerButton : MonoBehaviour
{
    private bool spawning = true;
    private bool pressed = false;
    private float speed = 1.0f;
    private float timer = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 pos = transform.position;
        pos.y -= 1.0f;
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        //button rises on start
        if (spawning)
        {
            timer -= Time.deltaTime;
            Vector3 pos = transform.position;
            pos.y += speed * Time.deltaTime;
            transform.position = pos;
            if (timer <= 0.0f)
            {
                timer = 1.0f;
                spawning = false;
            }
        }
        //if button is pressed, sink into ground for set timer
        if (pressed)
        {
            timer -= Time.deltaTime;
            Vector3 pos = transform.position;
            pos.y -= speed * Time.deltaTime;
            transform.position = pos;
            if (timer <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if hand collided begin the countdown and set 'pressed' to true
        if (other.tag == "Hand")
        {
            if (RoundManager.Instance.GetRoundOver())
            {
                RoundManager.Instance.StartRound();
                Destroy(gameObject);
            }
            else
            {
                RoundManager.Instance.BeginCountdown();
                pressed = true;
            }
        }
    }
    private void OnMouseDown()
    {
        if (RoundManager.Instance.GetRoundOver())
        {
            RoundManager.Instance.StartRound();
            Destroy(gameObject);
        }
        else
        {
            RoundManager.Instance.BeginCountdown();
            pressed = true;
        }
    }
}
