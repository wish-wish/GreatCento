using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cowbackForth : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	public float speed = -100f; //- = counterclockwise
	public float upDownSpeedCow = 1f;
	public float upDownRangeCow = 1f;
	public bool shouldCow = true;

	// Update is called once per frame
	void Update () { 
		if (shouldCow == true) 
		{
			transform.Rotate (new Vector3 (0, Time.deltaTime * speed, 0)); //roatation

			float newPos = Mathf.PingPong(Time.time * upDownSpeedCow, upDownRangeCow); //up and down movement
			transform.position = new Vector3 (transform.position.x, newPos, transform.position.z);

		}
	}
}
