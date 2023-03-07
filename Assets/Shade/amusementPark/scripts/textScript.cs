using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textScript : MonoBehaviour {

	public GameObject obj;

	// Use this for initialization
	void Start () {
		obj.SetActive(false);
	}
		
	void OnTriggerEnter(Collider col)
	{
		StartCoroutine( ShowAndHide(obj, 3.0f) ); // 3 seconds
		GameObject a = GameObject.Find ("drone");
		a.GetComponent<droneSelect> ().collide = true;
	}

	IEnumerator ShowAndHide( GameObject go, float delay )
	{
		go.SetActive(true);
		yield return new WaitForSeconds(delay);
		go.SetActive(false);
	}

	void Update () {

	}
}
