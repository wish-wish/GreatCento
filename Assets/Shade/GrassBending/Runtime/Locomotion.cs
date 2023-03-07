﻿using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Locomotion : MonoBehaviour
{
    private static readonly int moveSpeedId = Animator.StringToHash("MoveSpeed");

    private Animator animator;
    private Vector3 positionLastFrame;

    private void Awake ()
    {
        animator = GetComponent<Animator>();
    }

    private void Start ()
    {
        positionLastFrame = transform.position;
    }

    private void Update ()
    {
        var distance = Vector3.Distance(transform.position, positionLastFrame);
        animator.SetFloat(moveSpeedId, distance * Time.deltaTime);
        positionLastFrame = transform.position;
    }
}
