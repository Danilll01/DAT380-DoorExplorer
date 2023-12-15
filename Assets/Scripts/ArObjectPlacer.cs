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

    [SerializeField] private DoorSelector doorSelector;
    
    public void SpawnDoor()
    {
        // Cast a normal ray from the center of the screen
        if(Physics.Raycast(arCameraTransform.position, arCameraTransform.forward, out RaycastHit hit))
        {
            // Spawn the door at the hit point
            GameObject door = Instantiate(doorPrefab, hit.point, Quaternion.Euler(0, 180 + arCameraTransform.rotation.y, 0));
            // Get the portal script from the door
            Portal doorPortalScript = door.GetComponentInChildren<Portal>();
            // Set the linked portal to the portal we want to link to
            Portal framePortalScript = doorSelector.GetPortalScript();
            doorPortalScript.linkedPortal = framePortalScript;
            // Set the linked portal of the portal we want to link to to the portal we just spawned
            framePortalScript.linkedPortal = doorPortalScript;
        }
    }

    /*
    public void SpawnDoor()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;
            GameObject door = Instantiate(doorPrefab, hitPose.position, hitPose.rotation * Quaternion.Euler(0, 180, 0));
            Portal portalScript = door.GetComponentInChildren<Portal>();
            portalScript.linkedPortal = portalToLinkTo;
            portalToLinkTo.linkedPortal = portalScript;

        }
    }*/
}
