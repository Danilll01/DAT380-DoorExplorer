using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum Axis
{
    X,
    Y,
    Z
}

public class MechanicalHinge : MonoBehaviour
{
    [SerializeField] private bool isOpen = true;
    [SerializeField] private float speed = 5;
    [SerializeField] private Vector3 endAngle = new (0, 0, 90);
    [SerializeField] private Vector3 startAngle = new (0, 0, 0);
    [SerializeField] private Vector3 currentAngle = new (0, 0, 0);
    
    private void Start()
    {
        currentAngle = startAngle;
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H)) Open();
        
        // Dont lerp if we are close enough
        if (Vector3.Distance(transform.localRotation.eulerAngles, currentAngle) < 0.5f)
            return;
        //Vector3 lerpValue = Vector3.Lerp(transform.localRotation.eulerAngles, currentAngle, Time.deltaTime * 5);
        /*
        float x = 0;
        float y = 0;
        float z = 0;
        
        if(timeElapsed < lerpDuration)
        {
            Vector3 currRot = transform.localRotation.eulerAngles;
            x = Mathf.SmoothStep(currRot.x, currentAngle.x, timeElapsed / lerpDuration);
            y = Mathf.SmoothStep(currRot.y, currentAngle.y, timeElapsed / lerpDuration);
            z = Mathf.SmoothStep(currRot.z, currentAngle.z, timeElapsed / lerpDuration);
            
            
            transform.localRotation = Quaternion.Euler(x,y,z);
            timeElapsed += Time.deltaTime;
        }*/
        
        
        //float lerpValue = Mathf.SmoothDamp();
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(currentAngle), Time.deltaTime * speed);
    }
    
    public void Open()
    {
        if (isOpen)
        {
            currentAngle = startAngle;
            isOpen = false;
        }
        else
        {
            currentAngle = endAngle;
            isOpen = true;
        }
    }
    public void OnlyOpen()
    {
        currentAngle = endAngle;
        isOpen = true;
    }
    public void OnlyClose()
    {
        currentAngle = startAngle;
        isOpen = false;
    }
}
