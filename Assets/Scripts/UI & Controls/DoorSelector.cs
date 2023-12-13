using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSelector : MonoBehaviour
{
    [SerializeField] private Portal shedPortal;
    [SerializeField] private Portal housePortal;

    public static RoomType selectedRoomType = RoomType.Shed;
    
    public Portal GetPortalScript()
    {
        switch (selectedRoomType)
        {
            case RoomType.Shed:
                return shedPortal;
            case RoomType.House:
                return housePortal;
            default:
                return shedPortal;
        }
    }
}
