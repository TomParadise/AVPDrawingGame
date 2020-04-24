using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushWidthMenu : MonoBehaviour
{
    [SerializeField] private Transform aimTarget;//Target for the player to look at
    [SerializeField] private Transform cameraTransform;//Player camera transform

    [SerializeField] private Transform startPos;//Where the menu begins
    [SerializeField] private Transform targetPos;//Where the menu moves to

    [SerializeField] private SpriteRenderer[] buttons = new SpriteRenderer[3];//The 3 button sprites
    [SerializeField] private Sprite onSprite;//The sprite for inactive buttons
    [SerializeField] private Sprite offSprite;//The sprite for active buttons
    
    private float speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    //https://forums.oculusvr.com/developer/discussion/4198/tip-detecting-where-the-player-looks-at

        //calculate dot product to check if player is looking in the area of the palette
        float dot = 
            Vector3.Dot(
                (aimTarget.position - cameraTransform.position)
                .normalized,
                cameraTransform.forward);

        //if player is looking, move the width menu up and fade it in
        if(0.85f < dot && dot <= 1)
        {
            //move menu up and fade in
            transform.position = Vector3.Lerp(transform.position, targetPos.position, speed * Time.deltaTime);
            fade(1.0f);
        }
        //if player is not looking move the width menu down and fade it out
        else
        {
            //move menu down and fade away
            transform.position = Vector3.Lerp(transform.position, startPos.position, speed * Time.deltaTime);
            fade(0.0f);
        }
    }

    //fade towards the input target
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

    //set the 3 button sprites when one is selected
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
