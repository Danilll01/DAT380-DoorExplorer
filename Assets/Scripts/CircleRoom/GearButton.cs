using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implement the interface IInteractable
public class GearButton : MonoBehaviour, IInteractable
{
    private bool animationIsPlaying = false;
    [SerializeField] private float animationTimer = 0f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float animationDistance = 0.1f;
    [SerializeField] private PuzzleManager puzzleManager;
    [SerializeField] private int buttonNumber;

    private void Start()
    {
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
                puzzleManager.PressButton(buttonNumber);
                animationIsPlaying = false;
            }
        }
    }

    public void Interact()
    {
        animationIsPlaying = true;
    }
}