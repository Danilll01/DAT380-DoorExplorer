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
    private bool clearOutline;
    private float lastLength = 2f;
    private bool dropNextTeleport = false;

    [Header("Pick Up Settings")]
    [SerializeField] private float pickupDistance = 10.0f;
    [SerializeField] private float carryForce = 400.0f;
    [SerializeField] private float pickupDuration = 0.1f;
    [SerializeField] private float itemHolderDistance = 2f;
    [SerializeField] private float itemDropDistance = 1f;

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
            SelectItem();
        }
    }

    private RaycastHit SendRaycast(Vector3 start, Vector3 direction, float length)
    {
        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, length))
        {
            //Debug.DrawLine(start, start + direction * length, Color.green);
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
            CheckHolderBreak(0f);
            lastLength = 0f;
            itemHolder.transform.position = transform.position + transform.forward * itemHolderDistance;
            return;
        }

        if (hit.collider.tag == "Portal")
        {
            length = itemHolderDistance - Vector3.Distance(transform.position, hit.point);
            var inPortal = hit.collider.gameObject.transform.parent.parent.GetComponent<Portal>();
            Vector3 outPortalHitPos = inPortal.linkedPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(hit.point));
            Vector3 direction = (outPortalHitPos
             - inPortal.linkedPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(transform.position))).normalized;

            Debug.DrawLine(outPortalHitPos, outPortalHitPos + direction * length, Color.blue);
            itemHolder.transform.position = outPortalHitPos + direction * length;
            CheckHolderBreak(length);
            lastLength = length;
            return;
        }


        float distance = Vector3.Distance(transform.position, hit.point);
        itemHolder.transform.position = transform.position + transform.forward * itemHolderDistance;
        CheckHolderBreak(2f);
        lastLength = 2f;
    }

    private void CheckHolderBreak(float length)
    {
        /* if (heldItem != null)
        {
            if (Math.Abs(lastLength - length) > 0.1f)
            {
                Debug.Log("Change to great: " + Math.Abs(lastLength - length));
                DropItem();
            }
        } */
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

        //OutPortal is the portal that is closest to the itemholder
        Portal portal = ClosestPortal(itemHolder.transform.position);
        float portalShortcut = Vector3.Distance(itemHolder.transform.position, portal.transform.position)
        + Vector3.Distance(heldItem.transform.position, portal.linkedPortal.transform.position);
        float crowFlight = Vector3.Distance(itemHolder.transform.position, heldItem.transform.position);

        if (heldItem.GetComponent<PortalPhysicsObject>().hasTeleported
        && Vector3.Distance(itemHolder.transform.position, heldItem.transform.position) > 2f)
        {
            Debug.Log("Droped item");
            DropItem();
            return;
        }
        if (crowFlight > portalShortcut)
        {
            Debug.Log("Portal Shortcut");
            /* if (InsideNoGoZone(portal.linkedPortal, heldItem.transform.position))
            {
                Debug.Log("InsideNoGoZone");
                DropItem();
                return;
            } */
            if (BadTeleport())
            {
                Debug.Log("BadTeleport");
                DropItem();
            }
            else
            {
                heldItem.GetComponent<PortalPhysicsObject>().hasTeleported = false;
                Debug.DrawLine(heldItem.transform.position, heldItem.transform.position + DirectionThroughTeleport() * 5, Color.blue, 5f);
                heldItemRB.AddForce(DirectionThroughTeleport() * carryForce);
            }

        }
        else
        {
            Debug.Log("Crow Flight");
            Vector3 direction = (itemHolder.transform.position - heldItem.transform.position);
            float length = Vector3.Distance(itemHolder.transform.position, heldItem.transform.position);
            heldItem.GetComponent<PortalPhysicsObject>().hasTeleported = false;
            Vector3 itemholder_L = ClosestPortal(itemHolder.transform.position).transform.InverseTransformPoint(itemHolder.transform.position);
            Vector3 item_L = ClosestPortal(heldItem.transform.position).transform.InverseTransformPoint(heldItem.transform.position);
            Debug.Log("Crow - item_L: " + item_L + " itemholder_L: " + itemholder_L);
            heldItemRB.AddForce(direction * carryForce);

        }

    }

    private bool BadTeleport()
    {
        Vector3 itemholder_L = ClosestPortal(itemHolder.transform.position).transform.InverseTransformPoint(itemHolder.transform.position);
        Vector3 item_L = ClosestPortal(heldItem.transform.position).transform.InverseTransformPoint(heldItem.transform.position);
        Debug.Log("Shortcut - item_L: " + item_L + " itemholder_L: " + itemholder_L);
        return (item_L.z > 0 == itemholder_L.z > 0);
    }

    private Vector3 DirectionThroughTeleport()
    {
        Portal targetPortal = ClosestPortal(heldItem.transform.position);
        Vector3 itemholder_L = ClosestPortal(itemHolder.transform.position).transform.InverseTransformPoint(itemHolder.transform.position);
        Vector3 itemholder_W = targetPortal.transform.TransformPoint(itemholder_L);
        Vector3 item_W = heldItem.transform.position;

        Vector3 direction = -(item_W - itemholder_W).normalized;
        return direction;
    }

    //Returns the portal that is closest to the position
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

    private void SelectItem()
    {
        clearOutline = true;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDistance))
        {
            if (hit.collider.tag == "Portal")
            {
                float length = pickupDistance - Vector3.Distance(transform.position, hit.point);
                var inPortal = hit.collider.gameObject.transform.parent.parent.GetComponent<Portal>();
                Vector3 outPortalHitPos = inPortal.linkedPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(hit.point));
                Vector3 direction = (outPortalHitPos
                 - inPortal.linkedPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(transform.position))).normalized;

                HitItem(outPortalHitPos, direction, length);
            }
            else
            {
                HitItem(transform.position, transform.forward, pickupDistance);
            }
        }
        if (clearOutline && currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }

    private void HitItem(Vector3 start, Vector3 direction, float length)
    {
        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, length))
        {
            Debug.DrawLine(start, start + direction * length, Color.green);
            if (hit.collider.tag == "Item")
            {
                if (currentOutline == null)
                {
                    currentOutline = hit.collider.gameObject.GetComponent<Outline>();
                    currentOutline.enabled = true;
                }
                else if (currentOutline.gameObject != hit.collider.gameObject)
                {
                    currentOutline.enabled = false;
                    currentOutline = hit.collider.gameObject.GetComponent<Outline>();
                    currentOutline.enabled = true;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    PickupItem(hit.collider.gameObject);
                }
                clearOutline = false;
            }
        }
    }

    private void PickupItem(GameObject item)
    {
        Debug.Log("PickupItem");

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
        dropNextTeleport = false;
        heldItemRB.useGravity = true;
        heldItemRB.drag = 1.0f;
        heldItemRB.constraints = RigidbodyConstraints.None;
        heldItem.layer = 0;
        heldItem.GetComponent<PortalPhysicsObject>().hasTeleported = false;
        heldItem = null;
        heldItemRB = null;
    }

}