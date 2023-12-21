using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GearPeg : MonoBehaviour
{
    [SerializeField] private GearPeg nextPeg;
    [SerializeField] private GearPeg beforePeg;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Gear"))
        {
            other.transform.parent = transform;
        }
    }
}
