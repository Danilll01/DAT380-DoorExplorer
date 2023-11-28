using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTeleporter : PortalTraveller
{
    new Rigidbody rigidbody;

    [SerializeField] private SphereCollider coll = null;

    [SerializeField] private Transform colliderFollow;
    [SerializeField] private float collOffsetZ = -2.4f;

    private void FixedUpdate()
    {
        if (coll != null)
        {
            coll.center = colliderFollow.localPosition + new Vector3(0,0,collOffsetZ);
        }
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        //rigidbody.velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(GetComponent<Rigidbody>().velocity));
        //rigidbody.angularVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(GetComponent<Rigidbody>().angularVelocity)); ;

    }

    public override void EnterPortalThreshold()
    {
        base.EnterPortalThreshold();
    }

    public override void ExitPortalThreshold()
    {
        base.ExitPortalThreshold();
    }


}
