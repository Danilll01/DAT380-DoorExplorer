using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class GearPeg : MonoBehaviour
{
    [SerializeField] private GearPeg nextPeg;
    [SerializeField] private GearPeg beforePeg;
    [SerializeField] private PegType pegType = PegType.MIDDLE;
    
    private enum PegType
    {
        START,
        MIDDLE,
        END
    }

    private int currentForce = 0;
    private float currentSpeed = 0f;
    private float startSpeed = 0;

    private bool hasGear = false;
    private bool hasDropped = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if (pegType == PegType.START)
        {
            startSpeed = 30;
            ReCalculateSpeed();
            hasGear = true;
        }
        
        if (pegType == PegType.END)
        {
            hasGear = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Return if it is not the specific type of collider we look for
        if (other.GetType() != typeof(SphereCollider)) { return; }
        
        // Return if the gear has been dropped right before
        if (hasDropped)
        { hasDropped = false; return;}
        
        GearScript gearScript = other.GetComponent<GearScript>();
        if (gearScript != null && !hasGear)
        {
            
            Transform transform1 = other.transform;
            Rigidbody gearBody = transform1.GetComponent<Rigidbody>();
            
            hasGear = true;
            transform1.parent = transform;
            transform1.localPosition = Vector3.zero; 
            currentForce += gearScript.GetTurnForce();
            gearBody.isKinematic = true;
            
            if (beforePeg != null || nextPeg != null)
            {
                beforePeg.AddCurrentForce(currentForce, this);
                nextPeg.SetNewSpeed(-currentSpeed);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GearScript gearScript = other.GetComponent<GearScript>();
        if (gearScript != null && hasGear)
        {
            Transform transform1 = other.transform;
            Rigidbody gearBody = transform1.GetComponent<Rigidbody>();
            
            // We can not remove gear if it is supposed to be still
            if (gearBody.isKinematic) { return; }
            
            if (beforePeg != null || nextPeg != null)
            {
                beforePeg.AddCurrentForce(-currentForce, this);
                nextPeg.SetNewSpeed(0);
            }
                
            transform1.parent = null;
            currentForce -= gearScript.GetTurnForce();
            hasGear = false;
            hasDropped = true;
        }
    }

    private void AddCurrentForce(int force, GearPeg from)
    {
        currentForce += force;
        
        if (pegType == PegType.MIDDLE)
        {
            beforePeg.AddCurrentForce(currentForce, this);
        }

        if (pegType == PegType.START)
        {
            ReCalculateSpeed();
            from.SetNewSpeed(currentSpeed);
        }
    }

    private void SetNewSpeed(float speed)
    {
        if (hasGear)
        {
            currentSpeed = speed;
            nextPeg.SetNewSpeed(-currentSpeed);
        }
    }

    private void ReCalculateSpeed()
    {
        currentSpeed = startSpeed - currentForce * 2;
    }
}
