using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorSelectorButton : MonoBehaviour
{
    [SerializeField] private RoomType roomType = RoomType.Shed;
    [SerializeField] private Color selectedColor;
    private Color unselectedColor;

    private void Start()
    {
        unselectedColor = GetComponent<Image>().color;
    }

    public void SelectRoomType()
    {
        if (roomType == DoorSelector.selectedRoomType)
        {
            DoorSelector.selectedRoomType = RoomType.None;
            GetComponent<Image>().color = unselectedColor;
        }
        else
        {
            DoorSelector.selectedRoomType = roomType;
            GetComponent<Image>().color = selectedColor;
        }
    }

    private void Update()
    {
        if (roomType != DoorSelector.selectedRoomType)
        {
            GetComponent<Image>().color = unselectedColor;
        }
        else
        {
            GetComponent<Image>().color = selectedColor;
        }
    }
}
