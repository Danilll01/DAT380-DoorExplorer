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
    
    public void SpawnDoor()
    {
        // Cast a normal ray from the center of the screen
        if(Physics.Raycast(arCameraTransform.position, arCameraTransform.forward, out RaycastHit hit))
        {
            Transform door = DoorSelector.GetPortalTransform();
            door.rotation = Quaternion.Euler(0, arCameraTransform.rotation.eulerAngles.y - 180, 0);
            door.position = hit.point;
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
