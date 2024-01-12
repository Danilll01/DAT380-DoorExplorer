using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ARCameraTeleporter : PortalTraveller
{
    [SerializeField] private Transform cameraOffset;

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        // Starter
        cameraOffset.rotation = Quaternion.identity;
    
        // Teleport player
        cameraOffset.position = pos - (transform.position - cameraOffset.position);
       
        // Rotate camera right
        fromPortal.rotation.ToAngleAxis(out float angle, out Vector3 axis);
        cameraOffset.RotateAround(transform.position, axis, -angle);
        
        //print("OffsetEuler: " + cameraOffset.rotation.eulerAngles);
        //cameraOffset.rotation.ToAngleAxis(out float angleOffset, out Vector3 axisOffset);
        
        //print("OffsetAngleAxis: " + angleOffset + ", " + axisOffset);
    }
}
