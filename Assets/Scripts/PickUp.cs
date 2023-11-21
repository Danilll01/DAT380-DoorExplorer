using System.Collections;
using UnityEngine;


// Göra hållaren mer bouncey
// Plocka upp items genom portalen
// Physics objects och carry...

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
        Vector3 pointPosition = transform.position + transform.forward * itemHolderDistance;
        itemHolder = new GameObject("ItemHolder");
        itemHolder.transform.position = pointPosition;
        itemHolder.transform.parent = transform;
    }

    void Update()
    {
        MoveItemHolder();
        MouseHover();
        LeftClick();
        if (heldItem != null && !coRoutineRunning)
        {
            CarryItem();
        }
        teleported = false;
    }

    private void MoveItemHolder()
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

                Vector3 relativePos = inPortal.transform.InverseTransformPoint(hit.point);
                relativePos = Quaternion.Euler(0.0f, 0.0f, 0.0f) * relativePos;
                Vector3 mirroredRelativePos = new Vector3(relativePos.x, relativePos.y, -relativePos.z);

                Vector3 pos = outPortal.transform.TransformPoint(mirroredRelativePos);
                float length = itemHolderDistance - Vector3.Distance(transform.position, hit.point);
                Debug.DrawLine(pos, pos + transform.forward * length, Color.blue);

                itemHolder.transform.position = pos + transform.forward * length;

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
    }

    private void teleportedHolder()
    {
        if (Vector3.Distance(itemHolder.transform.position, lastPosition) > 1.0f)
        {
            teleported = true;
            i = 3;
        }
    }

    private void DropItemDistance()
    {
        if (heldItem != null && !coRoutineRunning)
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, itemHolderDistance + 0.2f))
            {
                if (hit.collider.tag != "Portal" && Vector3.Distance(itemHolder.transform.position, lastPosition) > 1.0f)
                {
                    heldItem.transform.position = lastPosition;
                    /* DropItem(); */
                }
            }
        }
    }

    private void MouseHover()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupDistance))
        {
            Outline outline = hit.collider.GetComponent<Outline>();
            if (outline != null && heldItem == null)
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
    }

    private void CarryItem()
    {
        Debug.Log("Pos: " + heldItem.transform.position);
        if (teleported || i > 0)
        {
            Debug.Log("Teleported -------------------");
            Debug.Log("Före: " + heldItem.transform.position);
            heldItemRB.velocity = Vector3.zero;
            heldItem.transform.position = itemHolder.transform.position; //This fix this
            Debug.Log("Efter: " + heldItem.transform.position);
            Physics.SyncTransforms();
            i --;
        }
        if (Vector3.Distance(heldItem.transform.position, itemHolder.transform.position) > 0.1f)
        {
            Vector3 direction = (itemHolder.transform.position - heldItem.transform.position);
            //Debug.DrawRay(heldItem.transform.position, direction, Color.red);
            //Debug.Log(direction);
            heldItemRB.AddForce(direction * carryForce);
        }

        /* if (Vector3.Distance(itemHolder.transform.position, heldItem.transform.position) > itemDropDistance)
        {
            DropItem();
        } */

    }

    private void LeftClick()
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
    }

    private void PickupItem()
    {
        if (itemInFront != null)
        {
            heldItem = itemInFront.transform.GetComponent<Collider>().gameObject;
            heldItemRB = heldItem.GetComponent<Rigidbody>();
            //heldItem.GetComponent<PortalPhysicsObject>().isHeld = true; // Change

            heldItemRB.useGravity = false;
            heldItemRB.drag = 20.0f;
            heldItemRB.constraints = RigidbodyConstraints.FreezeRotation;
            heldItem.layer = 2;
            MoveObjectToCamera();
        }
    }

    private void DropItem()
    {
        heldItemRB.useGravity = true;
        heldItemRB.drag = 1.0f;
        heldItemRB.constraints = RigidbodyConstraints.None;
        heldItem.transform.parent = savedParent;
        heldItem.layer = 0;
        //heldItem.GetComponent<PortalPhysicsObject>().isHeld = false; // Change
        heldItem = null;
        heldItemRB = null;
    }

    private void MoveObjectToCamera()
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
    }
}