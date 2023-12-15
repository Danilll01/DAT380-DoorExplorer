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

    [SerializeField] private DoorSelector doorSelector;
    
    public void SpawnDoor()
    {
        // Cast a normal ray from the center of the screen
        if(Physics.Raycast(arCameraTransform.position, arCameraTransform.forward, out RaycastHit hit))
        {
            Transform door = doorSelector.GetPortalTransform();
            door.position = hit.point;
            door.rotation = Quaternion.Euler(0, arCameraTransform.rotation.eulerAngles.y, 0);
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
