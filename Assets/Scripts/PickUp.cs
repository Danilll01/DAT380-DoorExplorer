using System.Collections;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private GameObject heldItem;
    private Rigidbody heldItemRB;
    private bool coRoutineRunning = false;
    private GameObject itemInFront;
    private GameObject itemHolder;
    private Vector3 lastPosition;
    private Transform savedParent;
    private bool teleported = false;

    [Header("Pick Up Settings")]
    [SerializeField] private float pickupDistance = 50.0f;
    [SerializeField] private float carryForce = 400.0f;
    [SerializeField] private float pickupDuration = 0.1f;
    [SerializeField] private float itemHolderDistance = 2f;
    [SerializeField] private float itemDropDistance = 1f;
    int i = -1;

    private void Start()
    {
        CreateItemHolder();
        lastPosition = itemHolder.transform.position;
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

    private void MoveItemHolder()
    {
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

    private void TeleportCrack()
    {
        //Calculate if the vector between itemholder can't go through the portal
        DropItem();
    }

    private Vector3 TeleportDirection()
    {
        Portal targetPortal = ClosestPortal(heldItem.transform.position);
        Vector3 itemholder_L = ClosestPortal(itemHolder.transform.position).transform.InverseTransformPoint(itemHolder.transform.position);
        Vector3 item_L = targetPortal.transform.InverseTransformPoint(heldItem.transform.position);
        Vector3 itemholder_W = targetPortal.transform.TransformPoint(itemholder_L);
        Vector3 item_W = targetPortal.transform.TransformPoint(item_L);

        Vector3 direction = (item_W - itemholder_W).normalized;
        return direction;
    }

    private void MoveItem()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DropItem();
            return;
        }

        if (Vector3.Distance(itemHolder.transform.position, heldItem.transform.position)
            > Vector3.Distance(itemHolder.transform.position, ClosestPortal(itemHolder.transform.position).transform.position))
        {
            //Teleport
            heldItemRB.AddForce(TeleportDirection() * carryForce);
        }
        else
        {
            //Dont Teleport
            Vector3 direction = (itemHolder.transform.position - heldItem.transform.position);
            heldItemRB.AddForce(direction * carryForce);
        }

    }

    private Portal ClosestPortal(Vector3 position)
    {
        GameObject[] portalArray = GameObject.FindGameObjectsWithTag("Portal");
        Portal closestPortal = portalArray[0].GetComponent<Portal>();
        foreach (GameObject portal in portalArray)
        {
            if (Vector3.Distance(position, portal.GetComponent<Portal>().transform.position)
                < Vector3.Distance(position, closestPortal.transform.position))
            {
                closestPortal = portal.GetComponent<Portal>();
            }
        }
        return closestPortal;
    }

    private void SelectItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDistance))
        {
            if (hit.collider.tag == "Item")
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PickupItem(hit.collider.gameObject);
                }
                else
                {
                    hit.collider.GetComponent<Outline>().enabled = true;
                }
            }
        }
    }

    private void PickupItem(GameObject item)
    {
        heldItem = item.transform.GetComponent<Collider>().gameObject;
        heldItemRB = heldItem.GetComponent<Rigidbody>();
        //heldItem.GetComponent<PortalPhysicsObject>().enabled = false;

        heldItemRB.useGravity = false;
        heldItemRB.drag = 20.0f;
        heldItemRB.constraints = RigidbodyConstraints.FreezeRotation;
        heldItem.layer = 2;
        //MoveObjectToCamera();
    }

    private void DropItem()
    {
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

