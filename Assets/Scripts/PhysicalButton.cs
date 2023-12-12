using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalButton : MonoBehaviour
{
    private bool animationIsPlaying = false;
    [SerializeField] private float animationTimer = 0f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float animationDistance = 0.1f;
    [SerializeField] private GameObject mainDoorObject;
    

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
                Portal portalScript = mainDoorObject.GetComponentInChildren<Portal>();
                portalScript.linkedPortal.linkedPortal = null;
                portalScript.linkedPortal = null;
                Destroy(mainDoorObject);
                animationIsPlaying = false;
                animationTimer = 0f;
                transform.localPosition = Vector3.zero;
            }
        }
    }

    public void PressButton()
    {
        animationIsPlaying = true;
    }
}
