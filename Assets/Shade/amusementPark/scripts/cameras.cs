using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameras : MonoBehaviour {

	Camera mainCamera;
	public Camera duckCamera;
	public Camera pigCamera;
	public Camera cowCamera;
	public Camera playerCam;
	public Camera droneCam;

	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		mainCamera.enabled = true;
		duckCamera.enabled = false;
		pigCamera.enabled = false;
		cowCamera.enabled = false;
		playerCam.enabled = false;
		droneCam.enabled = false;
	}

	public void clickedDuckCam() //enables camera on duck
	{
		duckCamera.enabled = true;
		mainCamera.enabled = false;
		pigCamera.enabled = false;
		cowCamera.enabled = false;
		playerCam.enabled = false;
		droneCam.enabled = false;
	}

	public void clickedPigCam() //enables camera on pig
	{
		pigCamera.enabled = true;
		mainCamera.enabled = false;
		duckCamera.enabled = false;
		cowCamera.enabled = false;
		playerCam.enabled = false;
		droneCam.enabled = false;
	}

	public void clickedCowCam() //enables camera on cow
	{
		cowCamera.enabled = true;
		mainCamera.enabled = false;
		duckCamera.enabled = false;
		pigCamera.enabled = false;
		playerCam.enabled = false;
		droneCam.enabled = false;
	}

	public void clickedMainCam() //enables main camera
	{
		mainCamera.enabled = true;
		duckCamera.enabled = false;
		pigCamera.enabled = false;
		cowCamera.enabled = false;
		playerCam.enabled = false;
		droneCam.enabled = false;
	}

	public void clickedPlayerCam() //enables player's camera
	{
		playerCam.enabled = true;
		mainCamera.enabled = false;
		duckCamera.enabled = false;
		pigCamera.enabled = false;
		cowCamera.enabled = false;
		droneCam.enabled = false;
	}

	public void clickedDroneCam() //enables camera on drone
	{
		droneCam.enabled = true;
		playerCam.enabled = false;
		mainCamera.enabled = false;
		duckCamera.enabled = false;
		pigCamera.enabled = false;
		cowCamera.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
