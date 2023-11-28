using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    // All doors that should be controlled
    [SerializeField] private HingeJoint[] joints;
    private HingeJoint openJoint;
    
    // Start is called before the first frame update
    void Start()
    {
        // Setup motor
        JointMotor motor = new JointMotor
        {
            force = 10,
            targetVelocity = 250,
            freeSpin = true
        };

        foreach (HingeJoint joint in joints)
        {
            joint.motor = motor;
            joint.useMotor = false;
        }

        openJoint = joints[0];
    }

    // Update is called once per frame
    void Update()
    {
        foreach (HingeJoint joint in joints)
        {
            // Turn on/off motor to close non active doors
            if (joint.angle < 89 && !joint.Equals(openJoint))
            {
                joint.useMotor = true;
            }
            else
            {
                joint.useMotor = false;
            }

            // Change which joint is open if one is starting to open
            if (!(joint.velocity < 0) || !(joint.velocity < openJoint.velocity)) continue;
            openJoint = joint;
            openJoint.useMotor = false;
        }

        
    }
}
