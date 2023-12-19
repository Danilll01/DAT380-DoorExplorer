using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSelector : MonoBehaviour
{
    [SerializeField] private Transform shedDoorTransform;
    [SerializeField] private Transform houseDoorTransform;

    public static RoomType selectedRoomType = RoomType.None;
    
    public Transform GetPortalTransform()
    {
        switch (selectedRoomType)
        {
            case RoomType.Shed:
                return shedDoorTransform;
            case RoomType.House:
                return houseDoorTransform;
            default:
                return shedDoorTransform;
        }
    }
}
