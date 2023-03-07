using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateBase : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	public bool shouldRotate = true;
	public float platSpeed = 10f;

	// Update is called once per frame
	void Update () {
		if (shouldRotate) {
			transform.Rotate (new Vector3 (0, Time.deltaTime * platSpeed, 0)); //rotation
		}
	}
}
