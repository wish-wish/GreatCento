using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour {

    private float yTranslate;
    private float yRotate;
    private float translateAmp;
    private float rotateAmp;
    private float timer;

    private void Start()
    {
        yTranslate = 0;
        yRotate = 0;
        translateAmp = 0.025f;
        rotateAmp = 0.5f;
        timer = 0;
    }
    // Update is called once per frame
    void Update () {
        yTranslate = Mathf.Cos(timer) * translateAmp;
        yRotate = Mathf.Cos(timer) * rotateAmp;

        transform.position = new Vector3(transform.position.x, 
                                         transform.position.y + yTranslate,
                                         transform.position.z);
        transform.rotation = new Quaternion(transform.rotation.x,
                                            yRotate,
                                            transform.rotation.z,
                                            transform.rotation.w);
        timer += 0.1f;
	}

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(transform.parent.gameObject);
    }
}
