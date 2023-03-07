using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class groundSelect : MonoBehaviour {

	Camera cam;
	bool showButtonsGround = false;
	private bool done = false;
	private Material m;
	private Material n;
	Color c;
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
		GameObject x = GameObject.Find("wholeRide");
		x.GetComponent<rotateBase> ().shouldRotate = false;

		GameObject y = GameObject.Find ("duck");
		y.GetComponent<teacupRotate> ().shouldSpin = false;

		GameObject z = GameObject.Find ("pole");
		z.GetComponent<upDown> ().shouldOsc = false;

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
		GameObject x = GameObject.Find("wholeRide");
		x.GetComponent<rotateBase> ().shouldRotate = true;

		GameObject y = GameObject.Find ("duck");
		y.GetComponent<teacupRotate> ().shouldSpin = true;

		GameObject z = GameObject.Find ("pole");
		z.GetComponent<upDown> ().shouldOsc = true;

		GameObject a = GameObject.Find ("cowPole");
		a.GetComponent<cowbackForth> ().shouldCow = true;
	}

	void OnGUI()
	{
		if (showButtonsGround == true) 
		{
			if (GUI.Button(new Rect(10, 70, 50, 30), "Done")) {
				showButtonsGround = false;
				done = true;
				deselect (rend, n, c);
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
		if (Input.GetMouseButtonDown (0) && GUIUtility.hotControl == 0) {
			if (EventSystem.current.IsPointerOverGameObject ()) 
			{
			} 
			else 
			{
				RaycastHit hitInfo = new RaycastHit (); //cast a ray
				bool hit = Physics.Raycast (cam.ScreenPointToRay (Input.mousePosition), out hitInfo);

				if (hit && hitInfo.transform.gameObject.tag == "ground") { //if ray hit ground

					Renderer[] rs = hitInfo.collider.gameObject.GetComponents<Renderer> ();

					foreach (Renderer r in rs) {
							m = r.material;
							n = r.material;
							c = r.material.color;
							rend = r;

							m.color = Color.green; //turn color green to indicate selection
							r.material = m;

							stopMoving (); //stop movement
							showButtonsGround = true; //"turn on" buttons
						}
					}
			}

		}

	}
}
