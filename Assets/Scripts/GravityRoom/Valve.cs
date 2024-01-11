using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : MonoBehaviour, IInteractable
{
    public bool isOpen = false;
    private float targetRotation = 0;
    [SerializeField] private float speed = 5;
    [SerializeField] private Collider blockingCollider;

    private void Start()
    {
        blockingCollider.enabled = true;
    }

    public void TurnValve()
    {
        targetRotation = 180;
        isOpen = true;
        blockingCollider.enabled = false;
    }
    
    public void TurnValveBack()
    {
        targetRotation = 0;
        isOpen = false;
        blockingCollider.enabled = true;
    }

    private void Update()
    {
        // Dont lerp if we are close enough
        if (Math.Abs(transform.localRotation.x - targetRotation) < 0.5f)
            return;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(targetRotation, -90, 90), Time.deltaTime * speed);
    }

    public void Interact()
    {
        if (isOpen)
        {
            TurnValveBack();
        }
        else
        {
            TurnValve();
        }
    }
}
