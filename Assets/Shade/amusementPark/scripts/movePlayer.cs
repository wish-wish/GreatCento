using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movePlayer : MonoBehaviour {

	public float speed;
	public float Rspeed;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		speed = 1.5f;
		Rspeed = 8F;
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		GameObject c = GameObject.Find("wholeRide");
		if (c.GetComponent<cameras> ().playerCam.enabled) {
			float moveH = Input.GetAxis ("Horizontal"); //arrow keys to move
			float moveV = Input.GetAxis ("Vertical");
			Vector3 movement = new Vector3 (moveH, 0F, moveV);

			rb.AddForce (movement * speed);

			Vector3 v3 = new Vector3 (0.0f, Input.GetAxis ("Fire2"), 0.0f); //z and x keys to rotate
			transform.Rotate (v3 * Rspeed * Time.deltaTime); 
		}
	}
}
