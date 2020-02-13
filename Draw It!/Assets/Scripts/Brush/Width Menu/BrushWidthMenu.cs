using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushWidthMenu : MonoBehaviour
{
    public Transform aimTarget;
    public Transform cameraTransform;

    public Transform startPos;
    public Transform targetPos;

    public SpriteRenderer[] buttons = new SpriteRenderer[3];
    public Sprite onSprite;
    public Sprite offSprite;
    
    private float speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    //https://forums.oculusvr.com/developer/discussion/4198/tip-detecting-where-the-player-looks-at

        float dot = 
            Vector3.Dot(
                (aimTarget.position - cameraTransform.position)
                .normalized,
                cameraTransform.forward);

        if(0.85f < dot && dot <= 1)
        {
            //move menu up and fade in
            transform.position = Vector3.Lerp(transform.position, targetPos.position, speed * Time.deltaTime);
            fade(1.0f);
        }
        else
        {
            //move menu down and fade away
            transform.position = Vector3.Lerp(transform.position, startPos.position, speed * Time.deltaTime);
            fade(0.0f);
        }
    }

    private void fade(float target)
    {
        Color tempcolor = gameObject.GetComponent<SpriteRenderer>().color;
        tempcolor.a = Mathf.MoveTowards(tempcolor.a, target, Time.deltaTime);
        gameObject.GetComponent<SpriteRenderer>().color = tempcolor;
        foreach (SpriteRenderer i in buttons)
        {
            i.color = tempcolor;
        }
    }

    public void setButtonSprites(SpriteRenderer Button)
    {
        foreach(SpriteRenderer i in buttons)
        {
            if(Button == i)
            {
                i.sprite = onSprite;
            }
            else
            {
                i.sprite = offSprite;
            }
        }
    }
}
