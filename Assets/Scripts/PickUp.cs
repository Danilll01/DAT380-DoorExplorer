using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private GameObject heldItem;
    private Rigidbody heldItemRB;

    [Header("Pick Up Settings")]
    [SerializeField] private float pickupDistance = 50.0f;
    [SerializeField] private float pickupForce = 20.0f;
    [SerializeField] private float pickupDuration = 0.25f;

    private bool itemCanFollow = false;
    private float carryDistance = 2.0f;

    void Update()
    {
        LeftClick();
        if (heldItem != null && itemCanFollow)
        {
            CarryItem();
        }
    }

    private void CarryItem()
    {
        Vector3 holdArea = transform.position + transform.forward * carryDistance;
        if (Vector3.Distance(heldItem.transform.position, holdArea) > 0.01f)
        {
            Vector3 direction = (holdArea - heldItem.transform.position);
            heldItemRB.AddForce(direction * pickupForce);
        }
    }
    private void LeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldItem == null)
            {
                PickupItem();
            }
            else
            {
                DropItem();
            }

        }
    }

    private void PickupItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupDistance))
        {
            if (hit.collider.GetComponent<PickAble>() != null)
            {
                heldItem = hit.collider.gameObject;
                heldItemRB = heldItem.GetComponent<Rigidbody>();

                heldItemRB.useGravity = false;
                heldItemRB.drag = 20.0f;
                heldItemRB.constraints = RigidbodyConstraints.FreezeRotation;

                Debug.Log("Selected Object: " + heldItem.name);
                MoveObjectToCamera();
            }
        }
    }

    private void DropItem()
    {
        heldItemRB.useGravity = true;
        heldItemRB.drag = 1.0f;
        heldItemRB.constraints = RigidbodyConstraints.None;
        itemCanFollow = false;
        heldItem = null;
        heldItemRB = null;
    }

    private void MoveObjectToCamera()
    {
        StopCoroutine("MoveObjectCoroutine");
        heldItemRB.angularVelocity = Vector3.zero;
        heldItemRB.velocity = Vector3.zero;
        StartCoroutine(MoveObjectToCameraCoroutine());
    }

    private IEnumerator MoveObjectToCameraCoroutine()
    {
        Vector3 start = heldItem.transform.position;
        float elapsed = 0f;

        while (elapsed < pickupDuration)
        {
            Vector3 pointInFront = transform.position + transform.forward * carryDistance;
            heldItem.transform.position = Vector3.Lerp(start, pointInFront, elapsed / pickupDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = transform.position + transform.forward * carryDistance;
        itemCanFollow = true;
    }


}
