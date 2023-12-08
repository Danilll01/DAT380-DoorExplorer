using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ArObjectPlacer : MonoBehaviour
{
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private ARPointCloudManager arPointCloudManager;
    [SerializeField] private Transform arCameraTransform; 
    [SerializeField] private GameObject doorPrefab;
    
    [SerializeField] private Portal portalToLinkTo;

    private void Update()
    {
        
    }

    public void SpawnDoor()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.FeaturePoint))
        {
            Pose hitPose = hits[0].pose;
            GameObject door = Instantiate(doorPrefab, hitPose.position, hitPose.rotation * Quaternion.Euler(0, 180, 0));
            Portal portalScript = door.GetComponentInChildren<Portal>();
            portalScript.linkedPortal = portalToLinkTo;
            portalToLinkTo.linkedPortal = portalScript;

        }
    }
}
