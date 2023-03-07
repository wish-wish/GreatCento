using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class droneMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
		speed = 1.5f;
		Rspeed = 8F;
		rb = GetComponent<Rigidbody> ();
	}

	public float speed;
	public float Rspeed;
	private Rigidbody rb;
	public float moveH = 0f;
	public float moveV = 0f;
	
	// Update is called once per frame
	void Update () {
			Vector3 movement = new Vector3 (moveH, 0F, moveV);

			rb.AddForce (movement * speed);

			float moveUp = Input.GetAxis ("Fire2"); //z and x keys to move up and down
			Vector3 moveUpDown = new Vector3 (0f, moveUp, 0f);
			rb.AddForce (moveUpDown * speed);

			float keyRotate = Input.GetAxis ("Fire1");
			if (keyRotate > 80f || keyRotate < -80f) 
			{
				keyRotate = 0;
			}
			Vector3 v3 = new Vector3 (0.0f, keyRotate, 0.0f); //a and s keys to rotate
			transform.Rotate (v3 * Rspeed * Time.deltaTime); 
	}
}
