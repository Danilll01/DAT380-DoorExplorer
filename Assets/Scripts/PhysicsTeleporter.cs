using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTeleporter : PortalTraveller
{


    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
        print("NU KÖR VI");
    }

    public override void EnterPortalThreshold()
    {
        //base.EnterPortalThreshold();
    }

    public override void ExitPortalThreshold()
    {
        //base.ExitPortalThreshold();
    }


}
