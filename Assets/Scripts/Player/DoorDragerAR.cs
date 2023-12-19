using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DoorDragerAR : MonoBehaviour
{
    [SerializeField] private LayerMask doorPortalLayer;

    [SerializeField] private float draggingForce = 100f;
    [SerializeField] private float pickupDistance = 3f;

    private bool holding = false;
    private Vector3 localHitDoor = Vector3.zero;
    private Transform doorTransform;
    private Rigidbody doorBody;
    private float hitDistance = 0;

    // Used for debugging
    //[SerializeField] private TextMeshProUGUI FOV;
    //[SerializeField] private TextMeshProUGUI farClip;
    //[SerializeField] private TextMeshProUGUI closeClip;
    //FOV.text = "FOV: " + GetComponent<Camera>().fieldOfView;
    //farClip.text = "FarClip: " + GetComponent<Camera>().farClipPlane;
    //closeClip.text = "CloseClip: " + GetComponent<Camera>().nearClipPlane;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !holding 
            || Input.touchCount > 0 && Input.GetTouch(0).phase.Equals(TouchPhase.Began) && !holding)
        {
            holding = false;
            RayCastStep(transform.position, transform.forward, pickupDistance,0);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0) || Input.touchCount > 0 && Input.GetTouch(0).phase.Equals(TouchPhase.Ended))
        {
            holding = false;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (!holding) return;

        Vector3 pullTo = RayCastStep(transform.position, transform.forward, hitDistance, 0);
        
        // Add force to door in the direction that the player is pulling
        Vector3 doorWorldDragPos = doorTransform.TransformPoint(localHitDoor);

        if (Vector3.Distance(pullTo, doorWorldDragPos) > pickupDistance)
        {
            holding = false;
            return;
        }
        
        Vector3 pullDirection = (pullTo - doorWorldDragPos);
        pullDirection *= draggingForce;
        doorBody.AddForceAtPosition(pullDirection * pullDirection.magnitude,doorWorldDragPos, ForceMode.Force);
    }

    private Vector3 RayCastStep(Vector3 origin, Vector3 direction, float distance, float totalDistance)
    {
        //Debug.DrawRay(origin, direction*distance, Color.cyan, 0.1f);
        
        // Cast rays if left mouse / touch is down
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, distance, doorPortalLayer))
        {
            Debug.DrawRay(origin, direction*distance, Color.cyan, 0.1f);
            
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Door"))
            {
                GetDoorInformation(hit, totalDistance);
                return (origin + direction * distance);
            }
            
            // Cast ray through portal in recursive step
            
            // Length left on other side
            float outPortalRayLength = distance - hit.distance;
            Portal inPortal = hit.collider.gameObject.transform.parent.GetComponent<Portal>();
            Transform outPortal = inPortal.linkedPortal.transform;
            Vector3 playerLocalPortalPosition = inPortal.transform.InverseTransformPoint(origin);
            Vector3 hitLocalPortalPosition = inPortal.transform.InverseTransformPoint(hit.point);
            Vector3 outPortalHitPos = outPortal.TransformPoint(hitLocalPortalPosition);
            Vector3 newRayDirection = (outPortalHitPos - outPortal.TransformPoint(playerLocalPortalPosition)).normalized;

            return RayCastStep(outPortalHitPos, newRayDirection, outPortalRayLength, hit.distance);
        }

        // We missed door
        return (origin + direction * distance);
    }

    // Get the information from a door hit
    private void GetDoorInformation(RaycastHit hit, float totalDistance)
    {
        doorBody = hit.transform.GetComponent<Rigidbody>();
        doorTransform = hit.transform;
        localHitDoor = hit.transform.InverseTransformPoint(hit.point);
        hitDistance = holding ? hitDistance : hit.distance + totalDistance;
        holding = true;
    }

    void OnDrawGizmos()
    {
        if (holding)
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(doorTransform.TransformPoint(localHitDoor), 0.5f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(RayCastStep(transform.position, transform.forward, hitDistance, 0), 0.5f);
        }
    }
}
