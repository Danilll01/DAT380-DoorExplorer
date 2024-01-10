using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSelector : MonoBehaviour
{
    [SerializeField] private static Transform shedDoorTransform;
    [SerializeField] private static Transform houseDoorTransform;
    [SerializeField] private static Transform gravityDoorTransform;

    public static RoomType selectedRoomType = RoomType.None;
    
    private void Start()
    {
        shedDoorTransform = transform.GetChild(0);
        houseDoorTransform = transform.GetChild(1);
        gravityDoorTransform = transform.GetChild(2);
    }
    
    public static Transform GetPortalTransform()
    {
        print(selectedRoomType.ToString());
        switch (selectedRoomType)
        {
            case RoomType.Shed:
                return shedDoorTransform;
            case RoomType.House:
                return houseDoorTransform;
            case RoomType.Gravity:
                return gravityDoorTransform;
            default:
                return shedDoorTransform;
        }
    }
}
