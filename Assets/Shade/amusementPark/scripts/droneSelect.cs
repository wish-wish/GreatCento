using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class droneSelect : MonoBehaviour {

	Camera cam;
	public bool showButtons = false;
	float hSliderValueSpeed = 1f;
	float hSliderValueRange = 1f;
	private bool done = false;
	private Material m;
	private Material n;
	private Texture t;
	private Color c;
	private Renderer rend;
	public Vector3 initialPos;
	public bool collide = false;

	// Use this for initialization
	void Start () {
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		initialPos = transform.position;
	}

/*
 * Function: clearSelection
 * ----------------------
 * replaces green color with original object's color
 *
 * Parameters: Renderer ren, Material y, Color col
 *
 * Returns: none
 */
	private void clearSelection(Renderer ren, Material y, Color col) 
	{
		y.color = col; //back to original color
		ren.material = y;
	}

	/*
 * Function: returnToPos() 
 * ----------------------
 * returns drone to starting position
 *
 * Parameters: 
 *
 * Returns: 
 */
	public void returnToPos()
	{
		
		transform.position = initialPos;
		GetComponent<droneMove> ().moveV = 0f;
		GetComponent<droneMove> ().moveH = 0f;
		collide = false;
	}

	void OnGUI()
	{
		if (showButtons == true) 
		{

			if (GUI.Button(new Rect(110, 150, 50, 30), "Done")) {
				showButtons = false;
				done = true;
				deselect (rend, n, c);
			}

			if (GUI.Button(new Rect(65, 40, 50, 30), "right")) {
				GetComponent<droneMove> ().moveH += 0.5f;
			}

			if (GUI.Button(new Rect(10, 40, 50, 30), "left")) {
				GetComponent<droneMove> ().moveH -= 0.5f;
			}

			if (GUI.Button(new Rect(35, 5, 60, 30), "forward")) {
				GetComponent<droneMove> ().moveV += 0.5f;
			}

			if (GUI.Button(new Rect(35, 75, 60, 30), "back")) {
				GetComponent<droneMove> ().moveV -= 0.5f;
			}

			if (GUI.Button(new Rect(25, 110, 85, 30), "Stop Moving")) { //add labels
				GetComponent<droneMove> ().moveV = 0f;
				GetComponent<droneMove> ().moveH = 0f;
			}

			if (GUI.Button(new Rect(5, 150, 95, 30), "Reset Position")) { //add labels
				returnToPos();
			}

		}
	}

/*
 * Function: deselect() 
 * ----------------------
 * deselects the object and calls clearSelection
 *
 * Parameters: Renderer r, Material n, Color c
 *
 * Returns: 
 */
	private void deselect (Renderer r, Material n, Color c){
		if (done == true) 
		{
			clearSelection (r, n, c); 
		}
	}

/*
 * Function: buttonSelect() 
 * ----------------------
 * selects the drone when the UI button is clicked
 *
 * Parameters: 
 *
 * Returns: 
 */
	public void buttonSelect()
	{
		Renderer r = GetComponent<Renderer> (); //select drone if UI button is pressed
		m = r.material;
		n = r.material;
		c = n.color;
		rend = r;
		m.color = Color.blue;
		r.material = m;
		showButtons = true;
	}


	// Update is called once per frame
	void Update () {

		if (collide)
			returnToPos();
	}
}

