using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teacupRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	public float speed = 100; 
	public bool shouldSpin = true;
	
	// Update is called once per frame
	void Update () {
		if (shouldSpin == true) 
		{
			transform.Rotate (new Vector3 (0, Time.deltaTime * speed, 0)); //rotation
		}
	}
}
