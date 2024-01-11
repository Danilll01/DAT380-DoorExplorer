using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelButton : MonoBehaviour
{
    [SerializeField] private MechanicalHinge recordPlayerHinge;
    private bool animationIsPlaying = false;
    [SerializeField] private float animationTimer = 0f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float animationDistance = 0.1f;
    
    private void Update()
    {
        if (animationIsPlaying)
        {
            float yPos = Mathf.Lerp(0, -animationDistance, animationTimer / animationDuration);
            transform.localPosition = new Vector3(0,yPos, 0);
            if (animationTimer < animationDuration)
            {
                animationTimer += Time.deltaTime;
            }
            else
            {
                recordPlayerHinge.Open();
                animationIsPlaying = false;
                animationTimer = 0f;
                transform.localPosition = Vector3.zero;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SelfPropellingCar car = other.GetComponent<SelfPropellingCar>();
        if (car != null)
        {
            car.SetPushForward(false);
            animationIsPlaying = true;
        }
    }
}
