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
    private Vector3 localHitPlayer = Vector3.zero;
    private Vector3 localHitDoor = Vector3.zero;
    private Transform doorTransform;
    private Rigidbody doorBody;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !holding)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transForward.lookVector, out hit, 3, doorLayer))
            {
                Debug.DrawRay(transform.position, transForward.lookVector * hit.distance, Color.yellow);
                Debug.Log("Did Hit");

                doorBody = hit.transform.GetComponent<Rigidbody>();
                doorTransform = hit.transform;
                localHitPlayer = transform.InverseTransformPoint(hit.point);
                localHitDoor = hit.transform.InverseTransformPoint(hit.point);
                holding = true;
            }
            else
            {
                Debug.DrawRay(transform.position, transForward.lookVector * 1000, Color.blue);
                Debug.Log("Did not Hit");
                holding = false;
            }

            
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            holding = false;
        }

        print("Hold: " + holding);
        
        if (holding)
        {
            print("HOLDING!!!!!!!!!!!!!!: " + holding);
            Vector3 doorWorldDragPos = doorTransform.TransformPoint(localHitDoor);
            Vector3 pullDirection = (transform.position + transForward.lookVector.normalized * 2) - doorWorldDragPos;
            pullDirection = pullDirection.normalized * draggingForce;
            doorBody.AddForceAtPosition(pullDirection,doorWorldDragPos, ForceMode.Force);
        }
        
        
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
