using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfPropellingCar : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float maxDriveTime = 4f;
    [SerializeField] private bool forcePushForward = false;
    [SerializeField] private Transform puzzleButtonPosition;
    [SerializeField] private float pushForce = 0.05f;
    private Rigidbody rb;
    private float timer;
    private bool primedToDrive;
    
    void Start()
    {
        timer = maxDriveTime;
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (forcePushForward) return;
        
        if (rb.freezeRotation)
        {
            primedToDrive = true;
        }

        if (primedToDrive && !rb.freezeRotation)
        {
            transform.position += transform.forward * (Time.deltaTime * speed);
            timer -= Time.deltaTime;
            if (timer < 0f)
            {
                primedToDrive = false;
                timer = maxDriveTime;
            }
            
            // Compare the normal from the transform to the Vector.up and if the angle is above 45 degrees, we are on a slope.
            
            if (Vector3.Angle(transform.up, Vector3.up) > 45f)
            {
                primedToDrive = false;
                timer = maxDriveTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G)) forcePushForward = !forcePushForward;
        if (forcePushForward)
        {
            Vector3 forceDir;
            float angle = Vector3.SignedAngle(transform.forward, -puzzleButtonPosition.up, Vector3.up);
            print("Angle: " + angle);
            if (angle < -5.0F)
            {
                print("turn left");
                forceDir = -transform.right;
            }
            else if (angle > 5.0F)
            {
                print("turn right");
                forceDir = transform.right;
            }
            else
            {
                forceDir = Vector3.zero;
            }
            
            rb.AddForceAtPosition(forceDir * pushForce * Mathf.Abs(angle), transform.position + transform.forward * 1f);
            
            transform.position += transform.forward * (Time.fixedDeltaTime * speed/2f);
        }
    }
}
