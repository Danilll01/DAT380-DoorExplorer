using System.Collections;
using UnityEngine;
using System;


// The tag item should add the required components, like outline and rigidbody

public class PickUp : MonoBehaviour
{
    private GameObject heldItem;
    private Rigidbody heldItemRB;
    private GameObject itemHolder;
    private Vector3 lastPosition;
    private Outline currentOutline;
    private bool clearOutline;
    private bool teleportBreak = false;

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
        if (Input.GetKeyDown(KeyCode.T))
        {
            Time.timeScale = 0;
        }
        FakeUpdate();
    }

    void FakeUpdate()
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

    private void MoveItemHolder()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * itemHolderDistance, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out hit, itemHolderDistance))
        {
            if (hit.collider.tag == "Portal")
            {
                float length = itemHolderDistance - Vector3.Distance(transform.position, hit.point);
                var inPortal = hit.collider.gameObject.transform.parent.parent.GetComponent<Portal>();
                Vector3 outPortalHitPos = inPortal.linkedPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(hit.point));
                Vector3 direction = (outPortalHitPos
                 - inPortal.linkedPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(transform.position))).normalized;

                Debug.DrawLine(outPortalHitPos, outPortalHitPos + direction * length, Color.blue);
                itemHolder.transform.position = outPortalHitPos + direction * length;
            }
            else
            {
                float distance = Vector3.Distance(transform.position, hit.point);
                itemHolder.transform.position = transform.position + transform.forward * distance;
            }
            return;
        }
        itemHolder.transform.position = transform.position + transform.forward * itemHolderDistance;
    }

    private bool InsideNoGoZone(Portal portal, Vector3 checkPos)
    {
        float angle = 15f;
        float width = portal.GetComponent<Collider>().bounds.size.x * 2f;
        Vector3 checkPos_L = portal.transform.InverseTransformPoint(checkPos);
        float xLength = (Mathf.Abs(checkPos_L.x) - (Math.Abs(width) / 2));
        float maxHeight = Mathf.Tan(angle) * xLength;
        //float maxHeight = tan v = a/b
        if ((Math.Abs(checkPos_L.z) < maxHeight))
        {
            return false;
        }
        else
        {
            if ((Math.Abs(checkPos_L.x) < (Math.Abs(width) / 2)))
            {
                return false;
            }
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

        if (teleportBreak && heldItem.GetComponent<PortalPhysicsObject>().hasTeleported)
        {
            DropItem();
            return;
        }
        if (crowFlight > portalShortcut)
        {
            Debug.Log("Do Teleport");
            teleportBreak = false;
            if (InsideNoGoZone(portal.linkedPortal, heldItem.transform.position))
            {
                DropItem();
                return;
            }
            Debug.DrawLine(portal.transform.position, portal.transform.position + new Vector3(0, 5, 0) * 5f, Color.red);
            Debug.DrawLine(heldItem.transform.position, itemHolder.transform.position + DirectionThroughTeleport() * 3f, Color.red);
            heldItem.GetComponent<PortalPhysicsObject>().hasTeleported = false;
            heldItemRB.AddForce(DirectionThroughTeleport() * carryForce);
        }
        else
        {
            Vector3 direction = (itemHolder.transform.position - heldItem.transform.position);
            breakWhenTeleport(direction);
            float length = Vector3.Distance(itemHolder.transform.position, heldItem.transform.position);
            Debug.DrawLine(heldItem.transform.position, heldItem.transform.position + direction * length, Color.red);
            heldItem.GetComponent<PortalPhysicsObject>().hasTeleported = false;
            heldItemRB.AddForce(direction * carryForce);

        }

    }

    private void breakWhenTeleport(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(heldItem.transform.position, direction, out hit, 1f))
        {
            if (hit.collider.tag == "Portal")
            {
                teleportBreak = true;
                return;
            }
        }
        teleportBreak = false;
    }

    private Vector3 DirectionThroughTeleport()
    {
        Portal targetPortal = ClosestPortal(heldItem.transform.position);
        Vector3 itemholder_L = ClosestPortal(itemHolder.transform.position).transform.InverseTransformPoint(itemHolder.transform.position);
        Vector3 itemholder_W = targetPortal.transform.TransformPoint(itemholder_L);
        Vector3 item_W = heldItem.transform.position;

        Vector3 direction = -(item_W - itemholder_W).normalized;
        Debug.DrawLine(item_W, item_W + direction * 5, Color.red);
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
        teleportBreak = false;
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
        heldItemRB.useGravity = true;
        heldItemRB.drag = 1.0f;
        heldItemRB.constraints = RigidbodyConstraints.None;
        heldItem.layer = 0;
        heldItem = null;
        heldItemRB = null;
    }

}

/* private void MoveObjectToCamera()
    {
        StopCoroutine("MoveObjectCoroutine");
        heldItemRB.angularVelocity = Vector3.zero;
        heldItemRB.velocity = Vector3.zero;
        coRoutineRunning = true;
        StartCoroutine(MoveObjectToCameraCoroutine());
    }

    private IEnumerator MoveObjectToCameraCoroutine()
    {
        float elapsed = 0f;
        Vector3 start = heldItem.transform.position;

        while (elapsed < pickupDuration)
        {
            heldItem.transform.position = Vector3.Lerp(start, itemHolder.transform.position, (elapsed / pickupDuration));
            elapsed += Time.deltaTime; // Something weird with Time.deltaTime
            yield return null;

        }

        itemHolder.transform.position = heldItem.transform.position;
        savedParent = heldItem.transform.parent;
        //heldItemRB.transform.parent = itemHolder.transform;
        coRoutineRunning = false;
    } */

/* private void MoveItemHolder()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * itemHolderDistance;
        Debug.DrawRay(transform.position, forward, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, itemHolderDistance))
        {
            if (hit.collider.tag == "Portal")
            {
                var inPortal = hit.collider.gameObject.transform.parent.parent.GetComponent<Portal>();
                var outPortal = inPortal.linkedPortal;

                Vector3 playerPos = inPortal.transform.InverseTransformPoint(transform.position);
                Vector3 hitPos = inPortal.transform.InverseTransformPoint(hit.point);

                Vector3 worldPlayerPos = outPortal.transform.TransformPoint(playerPos);
                Vector3 worldHitPos = outPortal.transform.TransformPoint(hitPos);

                Vector3 direction = (worldHitPos - worldPlayerPos).normalized;

                float length = itemHolderDistance - Vector3.Distance(transform.position, hit.point);
                Debug.DrawLine(worldHitPos, worldHitPos + direction * length, Color.blue);

                itemHolder.transform.position = worldHitPos + direction * length;

                teleportedHolder();
                lastPosition = itemHolder.transform.position;
            }
            else
            {
                itemHolder.transform.position = transform.position + transform.forward * itemHolderDistance;
                teleportedHolder();
                DropItemDistance();
                lastPosition = itemHolder.transform.position;

            }
        }
        else
        {
            itemHolder.transform.position = transform.position + transform.forward * itemHolderDistance;
            teleportedHolder();
            DropItemDistance();
            lastPosition = itemHolder.transform.position;
        }
    } */

/* private void teleportedHolder()
{
    if (Vector3.Distance(itemHolder.transform.position, lastPosition) > 1.0f)
    {
        teleported = true;
        i = 3;
    }
} */

/* private void DropItemDistance()
{
    if (heldItem != null && !coRoutineRunning)
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, itemHolderDistance + 0.2f))
        {
            if (hit.collider.tag != "Portal" && Vector3.Distance(itemHolder.transform.position, lastPosition) > 1.0f)
            {
                heldItem.transform.position = lastPosition;
                DropItem();
            }
        }
    }
} */


/* 
    private void SetOutline(GameObject obj, Outline outline)
    {
        if (obj != itemInFront)
        {
            ClearOutline();
            itemInFront = obj;
            outline.enabled = true;
        }
    }

    private void ClearOutline()
    {
        if (itemInFront != null)
        {
            itemInFront.GetComponent<Outline>().enabled = false;
            itemInFront = null;
        }
    } */

/* private void CarryItem()
{
    if (teleported || i > 0)
    {
        heldItemRB.velocity = Vector3.zero;
        heldItem.transform.position = itemHolder.transform.position; //This fix this
        Physics.SyncTransforms();
        i--;
    }
    if (Vector3.Distance(heldItem.transform.position, itemHolder.transform.position) > 0.1f)
    {
        Vector3 direction = (itemHolder.transform.position - heldItem.transform.position);
        heldItemRB.AddForce(direction * carryForce);
    }

    if (Vector3.Distance(itemHolder.transform.position, heldItem.transform.position) > itemDropDistance)
    {
        DropItem();
    }

} */

/* private void LeftClick()
{
    if (Input.GetMouseButtonDown(0))
    {
        if (heldItem == null)
        {
            PickupItem();
        }
        else if (!coRoutineRunning)
        {
            DropItem();
        }
    }
} */

