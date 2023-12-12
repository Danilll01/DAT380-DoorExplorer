using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARRotation : MonoBehaviour
{
    public Transform rotationObject = null;
    [SerializeField] [Range(0, 1)] private float dampening = 0.9f;
    
    private Vector2 originalScreenPos = Vector2.zero;
    private Quaternion originalRot = Quaternion.identity;
    private Quaternion modifierRot = Quaternion.identity;
    private Quaternion rotationDelta = Quaternion.identity;
    private bool endRotation = false;
    private float dampeningAngle = 0;
    private Vector3 dampeningVector = Vector3.zero;
    private float totalDragDistance = 0;

    private bool failSafeTouchEnd = false;

    // Update is called once per frame
    void Update()
    {
        if (rotationObject == null) { return; }
        failSafeTouchEnd = true;
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            originalRot = rotationObject.rotation;
            modifierRot = Quaternion.identity;
            originalScreenPos = Input.GetTouch(0).position;
            endRotation = false;
            totalDragDistance = 0;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 coordinateDiff = Input.GetTouch(0).position - originalScreenPos;
            
            // Create correct rotation based on coordinate diff and camera rotation
            modifierRot = Quaternion.AngleAxis(coordinateDiff.y / 8, transform.right) *
                          Quaternion.AngleAxis(-coordinateDiff.x / 8, transform.up);
            rotationDelta = ((modifierRot * originalRot) * Quaternion.Inverse(rotationObject.rotation));
            rotationObject.rotation = modifierRot * originalRot;
            totalDragDistance += Vector2.Distance(originalScreenPos, Input.GetTouch(0).position);
        }
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            endRotation = true;
            rotationDelta.ToAngleAxis(out dampeningAngle, out dampeningVector);
        }

        if (endRotation)
        {
            dampeningAngle *= dampening;
            rotationObject.rotation = Quaternion.AngleAxis(dampeningAngle, dampeningVector) * rotationObject.rotation;
        }
    }

    public bool HasRotatedObject()
    {
        return totalDragDistance > 20;
    }
}
