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

    private int currentGear;
    
    // Start is called before the first frame update
    void Start()
    {
        currentGear = GetHashCode();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, currentSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("TJENA!!!!!!!!!!!!!!!!!!!!!!!!!!: " + other.gameObject.layer);
        GearScript gearScript = other.GetComponent<GearScript>();
        if (gearScript != null && currentGear != other.gameObject.GetHashCode())
        {
            print("Inne!!!!!!!!!!!!!!!!!!!");
            currentGear = other.gameObject.GetHashCode();
            Transform transform1 = other.transform;
            transform1.parent = transform;
            transform1.localPosition = Vector3.zero;
            currentForce += gearScript.GetTurnForce();
            other.transform.GetComponent<Rigidbody>().isKinematic = true;

            if (beforePeg != null || nextPeg != null)
            {
                beforePeg.AddCurrentForce(currentForce);
                nextPeg.SetNewSpeed(-currentSpeed);
            }
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
