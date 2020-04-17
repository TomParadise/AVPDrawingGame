using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LineRendererSettings : MonoBehaviour
{
    //Declare a LineRenderer to store the component attached to the GameObject. 
    [SerializeField] LineRenderer rend;

    //Settings for the LineRenderer are stored as a Vector3 array of points. Set up a V3 array to //Initialize in Start. 
    Vector3[] points;

    //Declare a panel to change.
    public GameObject panel;
    public Image img;
    public Button btn;
    public bool pause;

    //Start is called before the first frame update
    void Start()
    {
        img = panel.GetComponent<Image>();

        //Get the LineRenderer attached to the GameObject. 
        rend = gameObject.GetComponent<LineRenderer>();

        //Initialize the LineRenderer
        points = new Vector3[2];

        //Set the start point of the LineRenderer to the position of the GameObject. 
        points[0] = Vector3.zero;

        //Set the end point 20 units away from the GO on the Z axis (pointing forward)
        points[1] = transform.position + new Vector3(0, 0, 7.5f);

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
        Debug.DrawRay(ray.origin, ray.direction);
        bool hitBtn = false;

        if (Physics.Raycast(ray, out hit, layerMask))
        {
            points[1] = transform.InverseTransformPoint(hit.point);
            rend.startColor = Color.green;
            rend.endColor = Color.green;
            btn = hit.collider.gameObject.GetComponent<Button>();
            hitBtn = true; 
        }
        else
        {
            points[1] = transform.forward + new Vector3(0, 0, 7.5f);
            rend.startColor = Color.white;
            rend.endColor = Color.white;
            hitBtn = false;
        }

        rend.SetPositions(points);
        rend.material.color = rend.startColor;
        return hitBtn;
    }

    void Update()
    {
        AlignLineRenderer(rend);
        if (AlignLineRenderer(rend) && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) != 0)
        {
            btn.onClick.Invoke();
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
}
