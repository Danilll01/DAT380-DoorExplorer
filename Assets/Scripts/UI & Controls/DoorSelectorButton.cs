using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSelectorButton : MonoBehaviour
{
    [SerializeField] private RoomType roomType = RoomType.Shed;
    
    public void SelectRoomType()
    {
        DoorSelector.selectedRoomType = roomType;
    }
}
