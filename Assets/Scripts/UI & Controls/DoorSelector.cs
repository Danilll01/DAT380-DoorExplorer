using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSelector : MonoBehaviour
{
    private static Transform shedDoorTransform;
    private static Transform houseDoorTransform;
    private static Transform gravityDoorTransform;
    private static Transform kuggenDoorTransform;
    private static Transform skyIslandDoorTransform;

    public static RoomType selectedRoomType = RoomType.None;
    
    private void Start()
    {
        shedDoorTransform = transform.GetChild(0);
        houseDoorTransform = transform.GetChild(1);
        gravityDoorTransform = transform.GetChild(2);
        kuggenDoorTransform = transform.GetChild(3);
        skyIslandDoorTransform = transform.GetChild(4);
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
            case RoomType.Kuggen:
                return kuggenDoorTransform;
            case RoomType.SkyIsland:
                return skyIslandDoorTransform;
            default:
                return shedDoorTransform;
        }
    }
}
