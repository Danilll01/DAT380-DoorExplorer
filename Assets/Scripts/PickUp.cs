using System.Collections;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private GameObject heldItem;
    private Rigidbody heldItemRB;
    private bool itemCanFollow = false;
    private float carryDistance = 2.0f;
    private GameObject objectInFront;
    private float highest = 0.0f; // for debugging

    [Header("Pick Up Settings")]
    [SerializeField] private float pickupDistance = 50.0f;
    [SerializeField] private float pickupForce = 20.0f;
    [SerializeField] private float pickupDuration = 0.25f;

    void Update()
    {
        MouseHover();
        LeftClick();
        if (heldItem != null && itemCanFollow)
        {
            CarryItem();
        }
    }

    private void MouseHover()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDistance))
        {
            Outline outline = hit.collider.GetComponent<Outline>();
            if (outline != null && heldItem == null) // Don't highlight objects if we're holding something
            {
                SetOutline(hit.collider.gameObject, outline);
            }
            else
            {
                ClearOutline();
            }
        }
        else
        {
            ClearOutline();
        }
    }

    private void SetOutline(GameObject obj, Outline outline)
    {
        if (obj != objectInFront)
        {
            ClearOutline();
            objectInFront = obj;
            outline.enabled = true;
        }
    }

    private void ClearOutline()
    {
        if (objectInFront != null)
        {
            objectInFront.GetComponent<Outline>().enabled = false;
            objectInFront = null;
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
        if(heldItemRB.velocity.magnitude > highest){
            highest = heldItemRB.velocity.magnitude;
        }
        Debug.Log(heldItemRB.velocity.magnitude + " " + highest);

        //if(heldItemRB.velocity.magnitude > 0.01f)
        //{
        //    DropItem();
        //}
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
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDistance))
        {
            Outline outline = hit.collider.GetComponent<Outline>();
            if (outline != null)
            {
                heldItem = hit.collider.gameObject;
                heldItemRB = heldItem.GetComponent<Rigidbody>();

                heldItemRB.useGravity = false;
                heldItemRB.drag = 20.0f;
                heldItemRB.constraints = RigidbodyConstraints.FreezeRotation;

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