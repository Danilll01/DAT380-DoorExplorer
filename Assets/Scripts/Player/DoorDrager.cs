using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDrager : MonoBehaviour
{
    [SerializeField] private LayerMask doorLayer;

    [SerializeField] private PlayerPhysicsMovement transForward;
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transForward.transformForward, out hit, 3, doorLayer))
        {
            Debug.DrawRay(transform.position, transForward.transformForward * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transForward.transformForward * 1000, Color.blue);
            Debug.Log("Did not Hit");
        }
    }
}
