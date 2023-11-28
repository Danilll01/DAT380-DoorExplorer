using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfPropellingCar : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    [SerializeField] private float maxDriveTime = 4f;
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
            
            /*
            RaycastHit hit;
            if (Physics.Raycast(transform.position - new Vector3(1,2,0), -Vector3.up, out hit)) {
                // Calculate the angle of the slope.
                Debug.DrawRay(transform.position - new Vector3(1,2,0), -Vector3.up, Color.red, 10f);
                float angle = Vector3.Angle(hit.normal, transform.up);
                print("Angle: " + angle);
                // If the angle is greater than the max slope angle, we are on a slope.
                if (angle < 45f)
                {
                    transform.position += transform.forward * Time.deltaTime * speed;
                }
                timer -= Time.deltaTime;
                if (timer < 0f)
                {
                    primedToDrive = false;
                    timer = maxDriveTime;
                }
            }*/
        }
    }
}
