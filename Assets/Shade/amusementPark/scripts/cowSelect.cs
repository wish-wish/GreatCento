using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class cowSelect : MonoBehaviour {

	Camera cam;
	bool showButtons = false;
	float hSliderValue = -100f;
	float heightSlider = 1f;
	float rangeSlider = 1f;
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
		y.color = col; //back to original color
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

		GameObject a = GameObject.Find ("cowPole");
		a.GetComponent<cowbackForth> ().shouldCow = false;
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
		GameObject a = GameObject.Find ("cowPole");
		a.GetComponent<cowbackForth> ().shouldCow = true;
	}

	void OnGUI()
	{
		if (showButtons == true) 
		{
			hSliderValue = GUI.HorizontalSlider(new Rect(0, 15, 100, 30), hSliderValue, -500f, 500f); 
			GameObject a = GameObject.Find ("cowPole");
			a.GetComponent<cowbackForth> ().speed = hSliderValue;
			GUILayout.Label ("Speed of Rotation");

			heightSlider = GUI.HorizontalSlider(new Rect(0, 45, 100, 30), heightSlider, 0.2F, 5F); 
			GameObject b = GameObject.Find ("cowPole");
			b.GetComponent<cowbackForth> ().upDownSpeedCow = heightSlider;
			GUILayout.Label ("Speed of Movement");

			rangeSlider = GUI.HorizontalSlider(new Rect(0, 75, 100, 30), rangeSlider, 0.5F, 5F); 
			GameObject j = GameObject.Find ("cowPole");
			j.GetComponent<cowbackForth> ().upDownRangeCow = rangeSlider;
			GUILayout.Label ("Range of Movement");

			if (GUI.Button(new Rect(10, 100, 50, 30), "Done")) {
				showButtons = false;
				done = true;
				deselect (rend, n, c);
			}

			if (GUI.Button(new Rect(10, 135, 100, 30), "Reset Rotation")) { 
				hSliderValue = -100;
			}

			if (GUI.Button(new Rect(10, 170, 85, 30), "Reset Speed")) {
				heightSlider = 1f;
			}

			if (GUI.Button(new Rect(100, 170, 85, 30), "Reset Range")) {
				rangeSlider = 1f;
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
	void Update () {
		if (Input.GetMouseButtonDown (0) && GUIUtility.hotControl==0) 
		{
			if (EventSystem.current.IsPointerOverGameObject ()) 
			{
			} 
			else 
			{
				RaycastHit hitInfo = new RaycastHit (); //cast a ray
				bool hit = Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hitInfo);

				if (hit && hitInfo.collider.tag == "cow") { //if ray hits cow

					Renderer[] rs = hitInfo.collider.gameObject.GetComponents<Renderer> ();

					foreach (Renderer r in rs) 
					{
						m = r.material;
						n = r.material;
						c = n.color;
						rend = r;
						m.color = Color.green; //turn green to indicate selection
						r.material = m; 

						stopMoving (); //stop moving
						showButtons = true; //"turn on" GUI buttons
					}

				}
			}

		}

	}
}