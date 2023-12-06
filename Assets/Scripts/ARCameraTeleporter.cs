using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ARCameraTeleporter : PortalTraveller
{
    [SerializeField] private Transform cameraOffset;

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        cameraOffset.position = pos - transform.localPosition;
    }
}
