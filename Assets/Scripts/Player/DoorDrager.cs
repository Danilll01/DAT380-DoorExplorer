using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class DoorDrager : MonoBehaviour
{
    [FormerlySerializedAs("doorLayer")] [SerializeField] private LayerMask doorPortalLayer;
    [SerializeField] private PlayerPhysicsMovement transForward;

    [SerializeField] private float draggingForce = 100f;
    [SerializeField] private float pickupDistance = 3f;

    private bool holding = false;
    private Vector3 localHitDoor = Vector3.zero;
    private Transform doorTransform;
    private Rigidbody doorBody;
    private float hitDistance = 0;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !holding)
        {
            holding = false;
            RayCastStep(transform.position, transForward.lookVector, pickupDistance);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            holding = false;
        }


        if (!holding) return;

        Vector3 pullTo = RayCastStep(transform.position, transForward.lookVector, hitDistance);
        
        // Add force to door in the direction that the player is pulling
        Vector3 doorWorldDragPos = doorTransform.TransformPoint(localHitDoor);
        Vector3 pullDirection = (pullTo - doorWorldDragPos).normalized;
        pullDirection *= draggingForce;
        doorBody.AddForceAtPosition(pullDirection,doorWorldDragPos, ForceMode.Force);
    }

    private Vector3 RayCastStep(Vector3 origin, Vector3 direction, float distance)
    {
        // Cast rays if left mouse is down
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, distance, doorPortalLayer))
        {
            Debug.DrawRay(origin, direction, Color.cyan);
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Door"))
            {
                GetDoorInformation(hit);
                return (origin + direction * distance);
            }

            // Length left on other side
            float outPortalRayLength = distance - Vector3.Distance(origin, hit.point);
            Portal inPortal = hit.collider.gameObject.transform.parent.GetComponent<Portal>();
            Transform outPortal = inPortal.linkedPortal.transform;
            Vector3 playerLocalPortalPosition = inPortal.transform.InverseTransformPoint(origin);
            Vector3 hitLocalPortalPosition = inPortal.transform.InverseTransformPoint(hit.point);
            Vector3 outPortalHitPos = outPortal.TransformPoint(hitLocalPortalPosition);
            Vector3 newRayDirection = (outPortalHitPos - outPortal.TransformPoint(playerLocalPortalPosition)).normalized;

            return RayCastStep(outPortalHitPos, newRayDirection, outPortalRayLength);
        }

        // We missed door
        return (origin + direction * distance);
    }

    // Get the information from a door hit
    private void GetDoorInformation(RaycastHit hit)
    {
        doorBody = hit.transform.GetComponent<Rigidbody>();
        doorTransform = hit.transform;
        localHitDoor = hit.transform.InverseTransformPoint(hit.point);
        hitDistance = hit.distance;
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
            Gizmos.DrawSphere(RayCastStep(transform.position, transForward.lookVector, hitDistance), 0.5f);
        }
    }
}
