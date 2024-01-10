using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenRecordPlayer : MonoBehaviour
{
    private bool isOpen = true;
    private float targetAngle = 0;
    
    void Update()
    {
        // Dont lerp if we are close enough
        if (Mathf.Abs(transform.localRotation.z - targetAngle) < 0.5f)
            return;
        
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, targetAngle), Time.deltaTime * 5);
    }
    
    public void Open()
    {
        if (isOpen)
        {
            targetAngle = 0;
            isOpen = false;
        }
        else
        {
            targetAngle = 90;
            isOpen = true;
        }
    }
}
