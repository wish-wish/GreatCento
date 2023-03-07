/// <summary>
/// Developed by pier shaw piershaw@gmail.com
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementEvents : EventTrigger
{
    // how slow the rotation is 
    private const float friction = 0.89f;
    // this is the power of the users swipe
    private Vector2 dragDistStart, dragDistEnd;
    // the float value of the drag
    private float dragPower;
    // starts the drag
    private bool startDrag;
    // makes the drag smooth
    private float damping;
    // this parent object that rotates
    private GameObject rotatioObject;
    // this is the selection Object
    private GameObject selectionObject;
    // last name of selected
    private string lastName;
    // hit test of selected
    private RaycastHit2D hit;
    // present color of selected 
    private Color lastColor;
    // the global object that will send a brodcast to all evants
    private GameObject globalObject;
    // layerMask
    private const int layerMask = 5;
    // ray Cast distance
    private const float rayCastdistance = -500.0f;
    //ray Cast Min Depth
    private const float rayCastMinDepth = -1000.0f;
    //ray Cast Max Depth
    private const float rayCastMaxDepth = 1000.0f;
    // mainCamera
    private GameObject mainCamera;
    // canvas
    private GameObject canvas;
    //target for the selected objects to rotate
    private GameObject target;

    // Use this for initialization
    void Start () {
        rotatioObject = GameObject.Find("RotatioObject");
        mainCamera = GameObject.Find("Main Camera");
        globalObject = GameObject.Find("GlobalObject");
        canvas = GameObject.Find("Canvas");
        target = GameObject.Find("Target");
        mainCamera.transform.position = new Vector3(canvas.transform.position.x, canvas.transform.position.y, -1024.0f);
        target.transform.position = new Vector3(canvas.transform.position.x, canvas.transform.position.y, 7000);
    }

    void Update()
    {
        //starts the drag and slows it down after let go
        if (startDrag)
        {
            damping = dragPower;
            damping *= friction * Time.deltaTime;
            rotatioObject.transform.Rotate(new Vector3(damping, 0, 0));
        }
        
        // this will hit test the center selection
        hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector2(Input.mousePosition.x, Input.mousePosition.y), rayCastdistance, layerMask, rayCastMinDepth, rayCastMaxDepth);

        // changes the color and name of selected
        if (Input.GetMouseButtonDown(0))
        {

            if (hit.collider != null)
            {
             
                selectionObject = hit.collider.gameObject;
                lastName = selectionObject.name;
                selectionObject.GetComponent<TextMesh>().text = "<<Selectioned>>";
                Color newColor = new Color(255, 0, 0);
                selectionObject.GetComponent<TextMesh>().color = newColor;
                // this is the deleget 
                globalObject.GetComponent<GlobalObject>().selectionDelegate(lastName);

            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selectionObject != null)
            {
                selectionObject.GetComponent<TextMesh>().text = lastName;
                Color newColor = new Color(255, 255, 255);
                selectionObject.GetComponent<TextMesh>().color = newColor;
            }
        }
    
    }

    /// OnDrag when user touches screen or mouse 
    public override void OnDrag(PointerEventData data)
    {
        dragDistStart = data.delta;
        dragPower = data.delta.y;
        startDrag = true;

    }
    /// OnEndDrag when user lets go of touche or mouse 
    public override void OnEndDrag(PointerEventData data)
    {
        startDrag = false;
        dragDistEnd = data.delta;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (hit)
            Gizmos.DrawLine(hit.transform.position,new Vector3(hit.transform.position.x, hit.transform.position.y, hit.distance));
    }

}
