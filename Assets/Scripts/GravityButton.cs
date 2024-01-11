using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3 gravityDirection = Vector3.down;

    public void Interact()
    {
        GravityManager.gravityDirection = gravityDirection;
    }
}
