using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class pigSelect : MonoBehaviour {

	Camera cam;
	bool showButtons = false;
	float hSliderValueSpeed = 1f;
	float hSliderValueRange = 1f;
	private bool done = false;
	private Material m;
	private Material n;
	private Texture t;
	private Color c;
	private Renderer rend;

	// Use this for initialization
	void Start () {
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
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
		y.color = col; //going back to original color
		ren.material = y;
	}

/*
 * Function: stopMoving() 
 * ----------------------
 * stops objects from moving
 *
 * Parameters: 
 *
 * Returns: 
 */
	protected void stopMoving()
	{
		GameObject z = GameObject.Find ("pole");
		z.GetComponent<upDown> ().shouldOsc = false;
	}


/*
 * Function: startMoving() 
 * ----------------------
 * stops objects from moving
 *
 * Parameters: 
 *
 * Returns: 
 */
	protected void startMoving()
	{
		GameObject z = GameObject.Find ("pole");
		z.GetComponent<upDown> ().shouldOsc = true;
	}

	void OnGUI()
	{
		if (showButtons == true) 
		{
			hSliderValueSpeed = GUI.HorizontalSlider(new Rect(0, 15, 100, 30), hSliderValueSpeed, 0.2F, 5F);
			GameObject a = GameObject.Find ("pole");
			a.GetComponent<upDown> ().upDownSpeed = hSliderValueSpeed;
			GUILayout.Label ("Speed of Movement");

			hSliderValueRange = GUI.HorizontalSlider(new Rect(0, 45, 100, 30), hSliderValueRange, 0.5F, 5F); 
			GameObject b = GameObject.Find ("pole");
			b.GetComponent<upDown> ().upDownRange = hSliderValueRange;
			GUILayout.Label ("Range of Movement");

			if (GUI.Button(new Rect(10, 70, 50, 30), "Done")) {
				showButtons = false;
				done = true;
				deselect (rend, n, c);
			}

			if (GUI.Button(new Rect(10, 105, 85, 30), "Reset Speed")) { 
				hSliderValueSpeed = 1f;
			}

			if (GUI.Button(new Rect(10, 140, 85, 30), "Reset Range")) {
				hSliderValueRange = 1f;
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
			startMoving ();
		}
	}


	// Update is called once per frame
	//parts of code taken from youtube tutorial "Unity Tutorial: 3 Ways to Indicate Selected Units | Part 1" by user quill18creates
	void Update () {
		if (Input.GetMouseButtonDown (0) && GUIUtility.hotControl==0) //if mouse is clicked and not pressing GUI button
		{
			if (EventSystem.current.IsPointerOverGameObject ()) 
			{
			} 
			else 
			{
				RaycastHit hitInfo = new RaycastHit (); //cast a ray
				bool hit = Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hitInfo);

				if (hit && hitInfo.collider.tag == "pig") { //if ray hit pig
					
					Renderer[] rs = hitInfo.collider.gameObject.GetComponentsInChildren<Renderer> ();

					foreach (Renderer r in rs) { //turn green to indicate selection
						m = r.material;
						n = r.material;
						c = n.color;
						rend = r;
						m.color = Color.green;
						r.material = m;

						stopMoving (); //stop moving
						showButtons = true; //"turn on" GUI buttons
					}

				}
			}

		}

	}
}
