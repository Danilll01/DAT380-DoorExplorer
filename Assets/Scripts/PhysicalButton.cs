using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implement the interface IInteractable
public class PhysicalButton : MonoBehaviour, IInteractable
{
    private bool animationIsPlaying = false;
    [SerializeField] private float animationTimer = 0f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float animationDistance = 0.1f;
    [SerializeField] private GameObject mainDoorObject;
    
    [SerializeField] private Transform doorTransform;
    private Vector3 originalDoorPosition;

    private void Start()
    {
        originalDoorPosition = doorTransform.position;
    }
    
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
                doorTransform.position = originalDoorPosition;
                animationIsPlaying = false;
                animationTimer = 0f;
                transform.localPosition = Vector3.zero;
            }
        }
    }

    public void Interact()
    {
        animationIsPlaying = true;
    }
}
