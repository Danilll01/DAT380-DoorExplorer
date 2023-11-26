using System.Collections;
using UnityEngine;
using System;


// The tag item should add the required components, like outline and rigidbody

public class PickUp : MonoBehaviour
{
    private GameObject heldItem;
    private Rigidbody heldItemRB;
    private GameObject itemHolder;
    private Outline currentOutline;
    private Vector3 forceDirection;

    [Header("Pick Up Settings")]
    [SerializeField] private float pickupDistance = 10.0f;
    [SerializeField] private float carryForce = 400.0f;
    [SerializeField] private float itemHolderDistance = 2f;

    private void Start()
    {
        CreateItemHolder();
    }

    private void CreateItemHolder()
    {
        itemHolder = new GameObject("ItemHolder");
        Vector3 pointPosition = transform.position + transform.forward * itemHolderDistance;
        itemHolder.transform.position = pointPosition;
        itemHolder.transform.parent = transform;
    }

    void Update()
    {
        MoveItemHolder();
        if (heldItem != null)
        {
            MoveItem();
        }
        else
        {
            SelectItem(transform.position, transform.forward, pickupDistance);
        }
    }

    void FixedUpdate() {
        if(heldItem != null)
        {
            float magnitude = forceDirection.magnitude;
            if(magnitude > 1) //Threshold for force
            {
                magnitude = 1;
            }
            float strength = carryForce * magnitude;
            heldItemRB.AddForce(forceDirection * strength);
        }
    }

    private RaycastHit SendRaycast(Vector3 start, Vector3 direction, float length)
    {
        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, length))
        {
            return hit;
        }
        return hit;
    }

    private void MoveItemHolder()
    {
        float length;
        RaycastHit hit = SendRaycast(transform.position, transform.forward, itemHolderDistance);


        if (hit.collider == null)
        {
            itemHolder.transform.position = transform.position + transform.forward * itemHolderDistance;
            return;
        }

        if (hit.collider.tag == "Portal")
        {
            length = itemHolderDistance - Vector3.Distance(transform.position, hit.point);
            var inPortal = hit.collider.gameObject.transform.parent.parent.GetComponent<Portal>();
            Debug.Log("Hit portal: " + inPortal.transform.InverseTransformPoint(hit.point));
            Vector3 outPortalHitPos = inPortal.linkedPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(hit.point));
            Vector3 direction = (outPortalHitPos
             - inPortal.linkedPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(transform.position))).normalized;

            Debug.DrawLine(outPortalHitPos, outPortalHitPos + direction * length, Color.blue);
            itemHolder.transform.position = outPortalHitPos + direction * length;
            return;
        }

        float distance = Vector3.Distance(transform.position, hit.point);
        itemHolder.transform.position = transform.position + transform.forward * itemHolderDistance;
    }

    private bool InsideNoGoZone(Portal portal, Vector3 checkPos)
    {
        float angle = 15f;
        float width = portal.GetComponent<Collider>().bounds.size.x * 2f;
        Vector3 checkPos_L = portal.transform.InverseTransformPoint(checkPos);
        float xLength = (Mathf.Abs(checkPos_L.x) - (Math.Abs(width) / 2));
        float maxHeight = Mathf.Tan(angle) * xLength;
        if ((Math.Abs(checkPos_L.z) < maxHeight) && (Math.Abs(checkPos_L.x) < (Math.Abs(width) / 2)))
        {
            return false;
        }
        else
        {
            Debug.Log("maxZ: " + maxHeight + " ,width: " + width / 2 + " ,xLength: " + xLength);
            Debug.Log("InsideNoGoZone: " + (Math.Abs(width) / 2) + " - " + (Math.Abs(checkPos_L.x)));
            return true;
        }

    }

    private void MoveItem()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DropItem();
            return;
        }

        Portal portal = ClosestPortal(itemHolder.transform.position);
        float portalShortcut = Vector3.Distance(itemHolder.transform.position, portal.transform.position)
        + Vector3.Distance(heldItem.transform.position, portal.linkedPortal.transform.position);
        Debug.DrawLine(heldItem.transform.position, portal.transform.position, Color.green);
        Debug.DrawLine(itemHolder.transform.position, portal.linkedPortal.transform.position, Color.green);
        float crowFlight = Vector3.Distance(itemHolder.transform.position, heldItem.transform.position);

        if (crowFlight > portalShortcut)
        {
            Debug.Log("Portal Shortcut");
            if (BadTeleport())
            {
                Debug.Log("BadTeleport");
                DropItem();
            }
            else
            {
                heldItem.GetComponent<PortalPhysicsObject>().hasTeleported = false;
                Debug.DrawLine(heldItem.transform.position, heldItem.transform.position + DirectionThroughTeleport() * 5, Color.blue, 5f);
                forceDirection = DirectionThroughTeleport();
            }

        }
        else
        {
            Vector3 direction = (itemHolder.transform.position - heldItem.transform.position);
            forceDirection = direction;
        }

    }

    private bool BadTeleport()
    {
        Vector3 itemholder_L = ClosestPortal(itemHolder.transform.position).transform.InverseTransformPoint(itemHolder.transform.position);
        Vector3 item_L = ClosestPortal(heldItem.transform.position).transform.InverseTransformPoint(heldItem.transform.position);
        // If the item has teleported add or remove to z based on itemholder pos
        if (heldItem.GetComponent<PortalPhysicsObject>().hasTeleported)
        {
            if (itemholder_L.z > 0)
            {
                item_L = item_L - new Vector3(0, 0, 1.0f);
            }
            else
            {
                item_L = item_L + new Vector3(0, 0, 1.0f);
            }
        }
        Debug.Log("Shortcut - item_L: " + item_L + " itemholder_L: " + itemholder_L);
        return (item_L.z > 0 == itemholder_L.z > 0);
    }

    private Vector3 DirectionThroughTeleport()
    {
        Portal targetPortal = ClosestPortal(heldItem.transform.position);
        Vector3 itemholder_L = ClosestPortal(itemHolder.transform.position).transform.InverseTransformPoint(itemHolder.transform.position);
        Vector3 itemholder_W = targetPortal.transform.TransformPoint(itemholder_L);
        Vector3 item_W = heldItem.transform.position;

        Vector3 direction = -(item_W - itemholder_W);
        return direction;
    }

    private Portal ClosestPortal(Vector3 position)
    {
        GameObject[] portalArray = GameObject.FindGameObjectsWithTag("Portal");
        Portal closestPortal = portalArray[0].transform.parent.parent.GetComponent<Portal>();
        foreach (GameObject portal in portalArray)
        {
            Vector3 portalPos = portal.transform.parent.parent.GetComponent<Portal>().transform.position;
            Vector3 closestPortalPos = closestPortal.transform.position;
            if (Vector3.Distance(position, portalPos) < Vector3.Distance(position, closestPortalPos))
            {
                closestPortal = portal.transform.parent.parent.GetComponent<Portal>();
            }
        }
        return closestPortal;
    }

    private void SelectItem(Vector3 startPos, Vector3 direction, float length)
    {
        RaycastHit hit = SendRaycast(startPos, direction, length);
        Debug.DrawLine(startPos, startPos + direction * length, Color.red);

        if (hit.collider == null)
        {
            SmartOutLine(null);
            return;
        }
        if (hit.collider.tag == "Portal")
        {
            float smallExtra = 0.1f; // This is used to help the raycast after the portal
            float remainingLength = pickupDistance - Vector3.Distance(startPos, hit.point) + smallExtra;
            var inPortal = hit.collider.gameObject.transform.parent.parent.GetComponent<Portal>();
            var outPortal = inPortal.linkedPortal;
            Vector3 hitPosIn = inPortal.transform.InverseTransformPoint(hit.point);
            Vector3 hitPosOut = outPortal.transform.TransformPoint(hitPosIn);
            Vector3 startPosOut = outPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(startPos));

            if(hitPosIn.z < 0)
            {
                smallExtra = -smallExtra;
            }

            Vector3 directionToPortal = (hitPosOut - startPosOut).normalized;

            SelectItem(hitPosOut + new Vector3(0,0,smallExtra), directionToPortal, remainingLength);
            return;
        }
        if (hit.collider.tag == "Item")
        {
            SmartOutLine(hit.collider.gameObject);
            if (Input.GetMouseButtonDown(0))
            {
                PickupItem(hit.collider.gameObject);
            }
            return;
        }
        SmartOutLine(null);
    }

    private void SmartOutLine(GameObject itemHit)
    {
        if(itemHit == null)
        {
            if(currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
            return;
        }
        if(itemHit.GetComponent<Outline>() == null)
        {
            if(currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
            return;
        }
        if(currentOutline == null)
        {
            currentOutline = itemHit.GetComponent<Outline>();
            currentOutline.enabled = true;
            return;
        }
        if(currentOutline.gameObject != itemHit)
        {
            currentOutline.enabled = false;
            currentOutline = itemHit.GetComponent<Outline>();
            currentOutline.enabled = true;
            return;
        }

    }

    private void PickupItem(GameObject item)
    {
        currentOutline.enabled = false;
        heldItem = item.transform.GetComponent<Collider>().gameObject;
        heldItemRB = heldItem.GetComponent<Rigidbody>();

        heldItemRB.useGravity = false;
        heldItemRB.drag = 20.0f;
        heldItemRB.constraints = RigidbodyConstraints.FreezeRotation;
        heldItem.layer = 2;
    }

    private void DropItem()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }

        heldItemRB.useGravity = true;
        heldItemRB.drag = 1.0f;
        heldItemRB.constraints = RigidbodyConstraints.None;
        heldItem.layer = 0;
        heldItem.GetComponent<PortalPhysicsObject>().hasTeleported = false;
        heldItem = null;
        heldItemRB = null;
    }

}