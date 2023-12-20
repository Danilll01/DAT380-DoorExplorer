using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public static Vector3 gravityDirection = Vector3.down;
    
    private List<Rigidbody> gravityObjects = new List<Rigidbody>(); 
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Rigidbody gravityObject in gravityObjects)
        {
            if (gravityObject != null)
            {
                gravityObject.AddForce(gravityDirection * 9.81f, ForceMode.Acceleration);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if(!gravityObjects.Contains(rb)) gravityObjects.Add(rb);
            rb.useGravity = false;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            gravityObjects.Remove(rb);
            rb.useGravity = true;
        }
    }
}
