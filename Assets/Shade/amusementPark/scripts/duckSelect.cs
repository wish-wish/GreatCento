using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class duckSelect : MonoBehaviour {

	Camera cam;
	bool showButtons = false;
	float hSliderValue = 100f;
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
		GameObject y = GameObject.Find ("duck");
		y.GetComponent<teacupRotate> ().shouldSpin = false;
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
		GameObject y = GameObject.Find ("duck");
		y.GetComponent<teacupRotate> ().shouldSpin = true;
	}

	void OnGUI()
	{
		if (showButtons == true) 
		{
			hSliderValue = GUI.HorizontalSlider(new Rect(0, 15, 100, 30), hSliderValue, -500f, 500F); 
			GameObject a = GameObject.Find ("duck");
			a.GetComponent<teacupRotate> ().speed = hSliderValue;
			GUILayout.Label ("Speed of Rotation");
		
			if (GUI.Button(new Rect(10, 35, 50, 30), "Done")) {
				showButtons = false;
				done = true;
				deselect (rend, n, c);
			}

			if (GUI.Button(new Rect(10, 70, 100, 30), "Reset Rotation")) { //add labels
				hSliderValue = 100;
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

				if (hit && hitInfo.collider.tag == "duck") { //if ray hits a duck
					
					Renderer[] rs = hitInfo.collider.gameObject.GetComponentsInChildren<Renderer> ();

					foreach (Renderer r in rs) {
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