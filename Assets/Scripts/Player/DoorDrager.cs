using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorDrager : MonoBehaviour
{
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private PlayerPhysicsMovement transForward;

    [SerializeField] private float draggingForce = 100f;

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
            // Cast rays if left mouse is down
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transForward.lookVector, out hit, 3, doorLayer))
            {
                // Get all info we need
                doorBody = hit.transform.GetComponent<Rigidbody>();
                doorTransform = hit.transform;
                localHitDoor = hit.transform.InverseTransformPoint(hit.point);
                hitDistance = hit.distance;
                holding = true;
            }
            else
            {
                // We missed door
                holding = false;
            }

            
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            holding = false;
        }


        if (!holding) return;
        
        // Add force to door in the direction that the player is pulling
        Vector3 doorWorldDragPos = doorTransform.TransformPoint(localHitDoor);
        Vector3 pullDirection = ((transform.position + transForward.lookVector.normalized * hitDistance) - doorWorldDragPos).normalized;
        pullDirection *= draggingForce;
        doorBody.AddForceAtPosition(pullDirection,doorWorldDragPos, ForceMode.Force);


    }
    
    void OnDrawGizmos()
    {
        if (holding)
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(doorTransform.TransformPoint(localHitDoor), 0.5f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + transForward.lookVector.normalized * 2, 0.5f);
        }
        
    }
}
