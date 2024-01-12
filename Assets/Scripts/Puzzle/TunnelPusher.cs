using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelPusher : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SelfPropellingCar car = other.GetComponent<SelfPropellingCar>();
        if (car != null)
        {
            car.SetPushForward(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        SelfPropellingCar car = other.GetComponent<SelfPropellingCar>();
        if (car != null)
        {
            car.SetPushForward(true);
        }
    }
}
