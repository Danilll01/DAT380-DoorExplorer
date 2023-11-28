using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushWorm : MonoBehaviour
{
    [SerializeField] private Rigidbody[] snakeParts;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (Rigidbody part in snakeParts)
            {
                part.AddForce(transform.parent.forward * 20, ForceMode.Impulse); 
            }
        }
        
    }
}
