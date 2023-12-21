using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GearPeg : MonoBehaviour
{
    [SerializeField] private GearPeg nextPeg;
    [SerializeField] private GearPeg beforePeg;

    private int currentForce = 0;
    private float currentSpeed = 0f;

    private bool hasGear = false;
    private bool hasDropped = false;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, currentSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("TJENA!!!!!!!!!!!!!!!!!!!!!!!!!!: " + other.gameObject.layer);
       
        if (other.GetType() != typeof(SphereCollider)) { return; }
        
        if (hasDropped)
        { hasDropped = false; return;}
        
        GearScript gearScript = other.GetComponent<GearScript>();
        if (gearScript != null && !hasGear)
        {
            print("Inne!!!!!!!!!!!!!!!!!!!");
            
            Transform transform1 = other.transform;
            Rigidbody gearBody = transform1.GetComponent<Rigidbody>();


            hasGear = true;
            transform1.parent = transform;
            transform1.localPosition = Vector3.zero; 
            currentForce += gearScript.GetTurnForce();
            gearBody.isKinematic = true;
            
            if (beforePeg != null || nextPeg != null)
            {
                beforePeg.AddCurrentForce(currentForce);
                nextPeg.SetNewSpeed(-currentSpeed);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print("HEJDÅ");
        GearScript gearScript = other.GetComponent<GearScript>();
        if (gearScript != null && hasGear)
        {
            Transform transform1 = other.transform;
            Rigidbody gearBody = transform1.GetComponent<Rigidbody>();
            if (gearBody.isKinematic) { return; }
            
            print("HEJDÅ PÅ RIKTIGT DENNA GÅNG");
            
            if (beforePeg != null || nextPeg != null)
            {
                beforePeg.AddCurrentForce( -currentForce );
                nextPeg.SetNewSpeed(0);
            }
                
            transform1.parent = null;
            currentForce -= gearScript.GetTurnForce();
            hasGear = false;
            hasDropped = true;
        }
    }

    private void AddCurrentForce(int force)
    {
        currentForce += force;
        beforePeg.AddCurrentForce(currentForce);
    }

    private void SetNewSpeed(float speed)
    {
        currentSpeed = speed;
        nextPeg.SetNewSpeed(-currentSpeed);
    }
}
