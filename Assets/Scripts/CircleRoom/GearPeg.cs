using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class GearPeg : MonoBehaviour
{
    [SerializeField] private GearPeg nextPeg;
    [SerializeField] private GearPeg[] beforePegs;
    [SerializeField] private GearPeg[] blockedPegs;
    [SerializeField] private PegType pegType = PegType.MIDDLE;
    [SerializeField] private Vector3 rotationRelation;
    [SerializeField] private bool ignoreExtraMeshNext = false;

    [SerializeField] private TextMeshPro powerMeter;
    [SerializeField] private TextMeshPro statusIndicator;
    
    private enum PegType
    {
        START,
        MIDDLE,
        END
    }

    [SerializeField] private int currentForce = 0;
    [SerializeField] private float currentSpeed = 0f;
    private float startSpeed = 0;

    private int holdingHash = 0;
    private bool hasGear = false;
    private bool hasDropped = false;
    private int isBlockedBy = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (pegType == PegType.START)
        {
            startSpeed = 32;
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
        if (other.GetType() != typeof(SphereCollider) || isBlockedBy > 0) { return; }
        
        // Return if the gear has been dropped right before
        if (hasDropped) { hasDropped = false; return;}
        
        GearScript gearScript = other.GetComponent<GearScript>();
        if (gearScript != null && !hasGear)
        {
            
            Transform transform1 = other.transform;
            Rigidbody gearBody = transform1.GetComponent<Rigidbody>();
            
            hasGear = true;
            holdingHash = transform1.GetHashCode();
            transform1.parent = transform;
            transform1.localPosition = Vector3.zero;
            transform1.localRotation = Quaternion.identity; //Quaternion.Euler(Vector3.zero);
            currentForce += gearScript.GetTurnForce();
            gearBody.isKinematic = true;

            ChangeBlockedPegs(1);
            
            if (beforePegs != null || nextPeg != null)
            {
                foreach (GearPeg beforePeg in beforePegs)
                { beforePeg.AddCurrentForce(currentForce, this); }
                //nextPeg.SetNewSpeed(-currentSpeed);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GearScript gearScript = other.GetComponent<GearScript>();
        Transform transform1 = other.transform;
        if (gearScript != null && hasGear && transform1.GetHashCode() == holdingHash)
        {
            Rigidbody gearBody = transform1.GetComponent<Rigidbody>();
            
            // We can not remove gear if it is supposed to be still
            if (gearBody.isKinematic) { return; }
            
            if (beforePegs.Length != 0 || nextPeg != null)
            {
                foreach (GearPeg beforePeg in beforePegs)
                { beforePeg.AddCurrentForce(-currentForce, this); }
                nextPeg.SetNewSpeed(0);
            }
                
            transform1.parent = null;
            currentForce -= gearScript.GetTurnForce();
            hasGear = false;
            holdingHash = 0;
            hasDropped = true;
            ChangeBlockedPegs(-1);
        }
    }

    private void AddCurrentForce(int force, GearPeg from)
    {
        currentForce += force;
        
        if (hasGear && pegType == PegType.MIDDLE)
        {
            foreach (GearPeg beforePeg in beforePegs)
            { beforePeg.AddCurrentForce(force, this); }
        }

        if (pegType == PegType.START)
        {
            ReCalculateSpeed();
            from.SetNewSpeed(-currentSpeed, transform.rotation.eulerAngles);
        }
    }

    private void SetNewSpeed(float speed, Vector3 newRotation = default, bool ignoreExtraRotation = false)
    {
        if (!hasGear) return;
        currentSpeed = speed;
        MeshGear(speed, newRotation, ignoreExtraRotation);

        if (pegType == PegType.MIDDLE)
        {
            nextPeg.SetNewSpeed(-currentSpeed, transform.rotation.eulerAngles, ignoreExtraMeshNext);
        }

        if (pegType == PegType.END)
        {
            statusIndicator.SetText(Mathf.Abs(speed) > 0 ? "Status:\nSpinning" : "Status:\nOffline");
        }
    }

    private void MeshGear(float speed, Vector3 newRotation, bool ignoreExtraRotation)
    {
        if (newRotation != default && Mathf.Abs(speed) > 0)
        {
            transform.rotation = Quaternion.Euler(-newRotation + (ignoreExtraRotation ? Vector3.zero : rotationRelation));
        }
    }

    private void SetBlockedStatus(int blocked)
    {
        isBlockedBy += blocked;
        isBlockedBy = Mathf.Max(0, isBlockedBy);
    }

    private void ReCalculateSpeed()
    {
        currentSpeed = currentForce <= 11 ? Mathf.Clamp(startSpeed - currentForce * 2f, 0f, startSpeed) : 0;

        if (powerMeter != null)
        {
            powerMeter.SetText("Power:\n" + currentForce + "/11");
        }
    }

    private void ChangeBlockedPegs(int block)
    {
        if (blockedPegs == null) return;
        foreach (GearPeg blockedPeg in blockedPegs)
        {
            blockedPeg.SetBlockedStatus(block);
        }
    }
}
