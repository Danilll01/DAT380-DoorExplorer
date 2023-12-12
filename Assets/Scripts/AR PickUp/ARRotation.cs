using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ARRotation : MonoBehaviour
{
    [SerializeField] private Transform rotationObject;

    private Vector2 originalScreenPos = Vector2.zero;
    private Quaternion originalRot = Quaternion.identity;
    private Quaternion modifierRot = Quaternion.identity;
    
    // Update is called once per frame
    void Update()
    {
        if (rotationObject.Equals(null)) { return; }
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            originalRot = rotationObject.rotation;
            modifierRot = Quaternion.identity;
            originalScreenPos = Input.GetTouch(0).position;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 coordinateDiff = Input.GetTouch(0).position - originalScreenPos;
            
            // Create correct rotation based on coordinate diff and camera rotation
            modifierRot = Quaternion.AngleAxis(coordinateDiff.y / 8, transform.right) *
                          Quaternion.AngleAxis(-coordinateDiff.x / 8, transform.up);
            rotationObject.rotation =  modifierRot * originalRot;
            
        }
        
    }
}
