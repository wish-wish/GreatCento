using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class upDown : MonoBehaviour {

	// Use this for initialization
	void Start () {
		upDownSpeed = 1f;
		upDownRange = 1f;
	}

	public float upDownSpeed = 1f;
	public float upDownRange = 1f;
	public bool shouldOsc = true;


	// Update is called once per frame
	void Update () {
		if (shouldOsc == true) 
		{
			float newPos = Mathf.PingPong(Time.time * upDownSpeed, upDownRange); //up and down movement
			transform.position = new Vector3 (transform.position.x, newPos, transform.position.z);

		}
	}
}
