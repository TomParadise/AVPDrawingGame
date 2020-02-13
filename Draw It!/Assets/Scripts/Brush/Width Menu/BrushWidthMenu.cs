using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushWidthMenu : MonoBehaviour
{
    public GameObject aimTarget;
    public GameObject camera;

    public SpriteRenderer[] buttons = new SpriteRenderer[3];
    public Sprite onSprite;
    public Sprite offSprite;

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
                (aimTarget.transform.position - camera.transform.position)
                .normalized, 
                camera.transform.forward);
        if(0.72f < dot && dot <= 1)
        {
            //move menu up
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
