using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
public class PointCloudExample : MonoBehaviour
{
    ARPointCloudManager pointtracker;
    Dictionary <ulong, GameObject> myPrimitives;
    void Awake()
    {
        pointtracker = GetComponent<ARPointCloudManager>();
        myPrimitives = new Dictionary <ulong, GameObject>();
    }
    void OnEnable()
    {
        pointtracker.pointCloudsChanged += myEventHandler;
    }
    void OnDisable()
    {
        pointtracker.pointCloudsChanged-= myEventHandler;
    }
    void myEventHandler (ARPointCloudChangedEventArgs eventArgs)
    {
        foreach (ARPointCloud cld in eventArgs.added)
        {
            handleTracking (cld);
        }
        foreach (ARPointCloud cld in eventArgs.updated)
        {
            handleTracking (cld);
        }
    }
    void handleTracking (ARPointCloud cloud)
    {
        Vector3 pos;
        GameObject gob;
        List<Vector3> points;
        List<ulong> identifiers;
        points = new List<Vector3>();
        identifiers = new List<ulong>();
        foreach (Vector3 feature in cloud.positions) {
            points.Add (feature);
        }
        foreach (ulong id in cloud.identifiers) {
            identifiers.Add (id);
        }
        for (int i = 0; i < points.Count; i++) {
            ulong id = identifiers[i];
            if (myPrimitives.ContainsKey(id) == false) {
                gob = GameObject.CreatePrimitive (PrimitiveType.Sphere);
                gob.transform.position = points[i];
                gob.transform.localScale = new Vector3 (0.01f, 0.01f, 0.01f);
                myPrimitives[id] = gob;
            }
        }
    }
}