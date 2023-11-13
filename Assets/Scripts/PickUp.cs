using System.Collections;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private GameObject heldItem;
    private Rigidbody heldItemRB;
    private bool coRoutineRunning = false;
    private GameObject itemInFront;
    private GameObject itemHolder;

    [Header("Pick Up Settings")]
    [SerializeField] private float pickupDistance = 50.0f;
    [SerializeField] private float carryForce = 400.0f;
    [SerializeField] private float pickupDuration = 0.1f;
    [SerializeField] private float itemHolderDistance = 2f;

    private void Start(){
        CreateItemHolder();
    }

    private void CreateItemHolder(){
        Vector3 pointPosition = transform.position + transform.forward * itemHolderDistance;
        itemHolder = new GameObject("ItemHolder");
        itemHolder.transform.position = pointPosition;
        itemHolder.transform.parent = transform;
    }

    void Update()
    {
        itemHolder.transform.position = transform.position + transform.forward * itemHolderDistance;
        MouseHover();
        LeftClick();
        if (heldItem != null && !coRoutineRunning)
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
        if (Vector3.Distance(heldItem.transform.position, itemHolder.transform.position) > 0.1f)
        {
            Vector3 direction = (itemHolder.transform.position - heldItem.transform.position);
            heldItemRB.AddForce(direction * carryForce);
        }

        ForceDrop();

    }

    private void ForceDrop(){ // Change to Distance Break

    }

    private void LeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldItem == null)
            {
                PickupItem();
            }
            else if(!coRoutineRunning)
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

                heldItemRB.useGravity = false;
                heldItemRB.drag = 20.0f;
                heldItemRB.constraints = RigidbodyConstraints.FreezeRotation;
                MoveObjectToCamera();
            }
    }

    private void DropItem()
    {
        heldItemRB.useGravity = true;
        heldItemRB.drag = 1.0f;
        heldItemRB.constraints = RigidbodyConstraints.None;
        heldItem.transform.parent = null;
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
        heldItemRB.transform.parent = itemHolder.transform;
        coRoutineRunning = false;
    }
}