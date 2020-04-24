using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicator : MonoBehaviour
{
    private Camera mainCamera;
    private RectTransform indicator;
    private Image indicatorImage;
    private Canvas mainCanvas;
    public Sprite targetIconOnScreen;
    public Sprite targetIconOffScreen;
    [Space]
    [Range(0, 500)]
    public float edgeBuffer;
    public Vector3 targetIconScale;
    private bool onScreen;

    void Start()
    {
        mainCamera = Camera.main;
        mainCanvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();

        indicator = new GameObject().AddComponent<RectTransform>();
        indicator.transform.SetParent(mainCanvas.transform);
        indicator.localScale = targetIconScale;
        indicatorImage = indicator.gameObject.AddComponent<Image>();
        indicatorImage.sprite = targetIconOnScreen;
    }

    void Update()
    {
        UpdateTargetIconPosition();
    }

    private void UpdateTargetIconPosition()
    {
        Vector3 newPos = transform.position;
        newPos = mainCamera.WorldToViewportPoint(newPos);
        if (newPos.x > 1 || newPos.y > 1 || newPos.x < 0 || newPos.y < 0)
        {
            onScreen = false;
        }
        else
        {
            onScreen = true;
        }
        if (newPos.z < 0)
        {
            newPos.x = 1f - newPos.x;
            newPos.y = 1f - newPos.y;
            newPos.z = 0;
            newPos = Vector3Maximize(newPos);
        }
        newPos = mainCamera.ViewportToScreenPoint(newPos);
        newPos.x = Mathf.Clamp(newPos.x - 1650 / 2, -1650 / 2 + edgeBuffer, 1650 / 2 - edgeBuffer);
        newPos.y = Mathf.Clamp(newPos.y - 1780 / 2, -1780 / 2 + edgeBuffer, 1780 / 2 - edgeBuffer);
        newPos.z = 0;
        indicator.transform.localPosition = newPos;
        if (!onScreen)
        {
            //Show the target off screen icon
            indicatorImage.sprite = targetIconOffScreen;
            //Rotate the sprite towards the target object
            var targetPosLocal = mainCamera.transform.InverseTransformPoint(transform.position);
            var targetAngle = -Mathf.Atan2(targetPosLocal.x, targetPosLocal.y) * Mathf.Rad2Deg - 90;
            //Apply rotation
            indicator.transform.localEulerAngles = new Vector3(0, 0, targetAngle-90);
        }
        else
        {
            //Reset rotation to zero and swap the sprite to the "on screen" one
            indicator.transform.localEulerAngles = new Vector3(0, 0, 0);
            indicatorImage.sprite = targetIconOnScreen;
        }

    }

    public Vector3 Vector3Maximize(Vector3 vector)
    {
        Vector3 returnVector = vector;
        float max = 0;
        max = vector.x > max ? vector.x : max;
        max = vector.y > max ? vector.y : max;
        max = vector.z > max ? vector.z : max;
        returnVector /= max;
        return returnVector;
    }

    private void OnDestroy()
    {
        Destroy(indicator.gameObject);
    }
}
