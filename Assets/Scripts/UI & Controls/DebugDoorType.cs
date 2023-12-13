using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugDoorType : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;

    private void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        textField.text = "Selected door" + DoorSelector.selectedRoomType.ToString();
    }
}
