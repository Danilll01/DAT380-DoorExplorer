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
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            originalRot = transform.rotation;
            modifierRot = Quaternion.identity;
            originalScreenPos = Input.GetTouch(0).position;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 coordinateDiff = Input.GetTouch(0).position - originalScreenPos;
            //modifierRot.eulerAngles = new Vector3(coordinateDiff.y/8, -coordinateDiff.x/8);
            modifierRot = Quaternion.AngleAxis(coordinateDiff.y / 8, Vector3.right) *
                          Quaternion.AngleAxis(-coordinateDiff.x / 8, Vector3.up);
            transform.rotation =  modifierRot * originalRot;
            
        }
        
    }
}
