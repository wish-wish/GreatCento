using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private float xTranslate, zTranslate;

    public float xVelocity, zVelocity;

	// Use this for initialization
	void Start () {
        xTranslate = 0;
        zTranslate = 0;
	}
	
	// Update is called once per frame
	void Update () {
        xTranslate = Input.GetAxis("Horizontal") * Time.deltaTime * xVelocity;
        zTranslate = Input.GetAxis("Vertical") * Time.deltaTime * zVelocity;

        transform.Translate(xTranslate, 0, zTranslate);
	}
}
