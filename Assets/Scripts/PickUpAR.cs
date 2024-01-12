using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


// The tag item should add the required components, like outline and rigidbody

public class PickUpAR : MonoBehaviour
{
    private GameObject heldItem;
    private Rigidbody heldItemRB;
    private GameObject itemHolder;
    private Outline currentOutline;
    private Vector3 forceDirection;

    private Vector3 saved;
    private Vector3 saved2;
    private Portal saved3;
    private bool savedBool = false;
    private Vector3 saved4;

    private Vector3 backupHolder;
    private bool lastHit = false;
    private bool pickUpButtonClicked = false;
    private bool doorPlacingMode = false;
    private ArObjectPlacer arObjectPlacer;
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private Transform floor;
    [SerializeField] private Transform doorFrameOutline;
    private Vector3 doorFrameOutlineOriginalPosition;
    [SerializeField] private Vector3 doorFrameOffset = new Vector3(0.1f, 0, 0);
    //[SerializeField] private float maxDistanceFromCenter = 10f;

    [Header("Pick Up Settings")]
    [SerializeField] private float pickupDistance = 10.0f;
    [SerializeField] private float carryForce = 400.0f;
    [SerializeField] private float itemHolderDistance = 2f;
    
    [Header("Pick up helpers")] 
    [SerializeField] private ARRotation rotationObject;


    private void Start()
    {
        CreateItemHolder();
        arObjectPlacer = GetComponent<ArObjectPlacer>();
        doorFrameOutlineOriginalPosition = doorFrameOutline.position;
    }

    private void CreateItemHolder()
    {
        itemHolder = new GameObject("ItemHolder");
        Vector3 pointPosition = transform.position + transform.forward * itemHolderDistance;
        itemHolder.transform.position = pointPosition;
        itemHolder.transform.parent = transform;
        backupHolder = itemHolder.transform.position;
    }

    void Update()
    {
        // Checks for mouse clicks and if held object becomes kinematic (in this case the item should be dropped)
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            pickUpButtonClicked = true;
        }
#endif
        
        // Check if held item has been set to kinematic
        if (heldItem != null && heldItemRB.isKinematic)
        {
            pickUpButtonClicked = true;
        }
        
        doorPlacingMode = DoorSelector.selectedRoomType != RoomType.None; 
        
        MoveItemHolder();
        if (heldItem != null)
        {
            MoveItem();
        }
        else
        {
            SelectItem(transform.position, transform.forward, pickupDistance);
        }
        if (savedBool)
        {
            DebDraw();
        }
        pickUpButtonClicked = false;
    }

    void FixedUpdate()
    {
        if (heldItem != null)
        {
            float magnitude = forceDirection.magnitude;
            if (magnitude > 0.5f) //Threshold for force
            {
                magnitude = 0.5f;
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
        if (hit.collider != null) Debug.DrawLine(transform.position, hit.transform.position, Color.red);

        if (ContingencyPlan())
        {
            itemHolder.transform.position = backupHolder;
            //Debug.Log("ContingencyPlan");
            lastHit = false;
            return;
        }

        if (hit.collider == null)
        {
            itemHolder.transform.position = transform.position + transform.forward * itemHolderDistance;
            backupHolder = itemHolder.transform.position;
            lastHit = false;
            return;
        }

        if (hit.collider.tag == "Portal") //It goes through the portal sometimes
        {
            length = itemHolderDistance - Vector3.Distance(transform.position, hit.point);
            var inPortal = hit.collider.gameObject.transform.parent.GetComponent<Portal>();
            Vector3 outPortalHitPos = inPortal.linkedPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(hit.point));
            Vector3 direction = (outPortalHitPos
             - inPortal.linkedPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(transform.position))).normalized;

            Debug.DrawLine(outPortalHitPos, outPortalHitPos + direction * length, Color.blue);
            itemHolder.transform.position = outPortalHitPos + direction * length;
            backupHolder = itemHolder.transform.position;
            if (!lastHit)
            {
                //Debug.Log("Missed");
            }
            lastHit = true;
            return;
        }

        float distance = Vector3.Distance(transform.position, hit.point);
        itemHolder.transform.position = transform.position + transform.forward * itemHolderDistance;
        lastHit = false;
        backupHolder = itemHolder.transform.position;
    }

    private bool ContingencyPlan()
    {
        var portal = ClosestPortal(transform.position);
        Vector3 playerPos_L = portal.transform.InverseTransformPoint(transform.position);
        if (playerPos_L.x > -2.2f && playerPos_L.x < 2.2f)
        {
            if (playerPos_L.z > -0.21f && playerPos_L.z < 0.21f)
            {
                return true;
            }
        }
        return false;
    }

    private void DebDraw()
    {
        Debug.DrawLine(saved, saved3.linkedPortal.transform.position, Color.green); //
        Debug.DrawLine(saved2, saved3.transform.position, Color.green);
        Debug.DrawLine(saved, saved2, Color.red); //Normal
        Debug.DrawLine(saved2, saved4, Color.blue); //Normal
    }

    private void MoveItem()
    {
        if (DropItemCheck()) return;

        Portal portal = ClosestPortal(itemHolder.transform.position);
        float portalShortcut = Vector3.Distance(itemHolder.transform.position, portal.transform.position)
        + Vector3.Distance(heldItem.transform.position, portal.linkedPortal.transform.position);
        saved3 = portal;
        float crowFlight = Vector3.Distance(itemHolder.transform.position, heldItem.transform.position);

        if (crowFlight > portalShortcut)
        {
            Debug.Log("Portal Shortcut");
            if (BadTeleport())
            {
                Debug.Log("BadTeleport");
                saved = heldItem.transform.position;
                saved2 = itemHolder.transform.position;
                saved4 = transform.position;
                DebDraw();
                DropItem();
                savedBool = true;
            }
            else
            {
                Debug.DrawLine(heldItem.transform.position, itemHolder.transform.position, Color.green, 10f); //
                forceDirection = DirectionThroughTeleport();
            }

        }
        else
        {
            if (Vector3.Distance(itemHolder.transform.position, heldItem.transform.position) > 5f)
            {
                Debug.Log("Too far away");
                DropItem();
                return;
            }
            Vector3 direction = (itemHolder.transform.position - heldItem.transform.position);
            forceDirection = direction;
        }

    }

    private bool BadTeleport()
    {
        Vector3 itemholder_L = ClosestPortal(itemHolder.transform.position).transform.InverseTransformPoint(itemHolder.transform.position);
        Vector3 item_L = ClosestPortal(heldItem.transform.position).transform.InverseTransformPoint(heldItem.transform.position);
        // Summan av kaddemumman, Teleport blir kallad f�r sent
        // Ditt fuck detta funkar inte
        if (item_L.z > -0.2f && item_L.z < 0.2f)
        {
            Debug.Log("In the middle of the portal");
            return false;
        }
        if (itemholder_L.z > -0.2f && itemholder_L.z < 0.2f)
        {
            Debug.Log("In the middle of the portal");
            return false;
        }
        Debug.Log("Shortcut - item_L: " + item_L + " itemholder_L: " + itemholder_L + " player_L: " + ClosestPortal(transform.position).transform.InverseTransformPoint(transform.position));
        return ((item_L.z >= 0) == (itemholder_L.z > 0));
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
        Portal closestPortal = portalArray[0].transform.parent.GetComponent<Portal>();
        foreach (GameObject portal in portalArray)
        {
            Vector3 portalPos = portal.transform.parent.GetComponent<Portal>().transform.position;
            Vector3 closestPortalPos = closestPortal.transform.position;
            if (Vector3.Distance(position, portalPos) < Vector3.Distance(position, closestPortalPos))
            {
                closestPortal = portal.transform.parent.GetComponent<Portal>();
            }
        }
        return closestPortal;
    }

    private void SelectItem(Vector3 startPos, Vector3 direction, float length)
    {
        if (doorPlacingMode)
        {
            HandleDoorPlacement(startPos, direction);
            return;
        }
        
        RaycastHit hit = SendRaycast(startPos, direction, length);
        if (hit.collider == null)
        {
            SmartOutLine(null);
            return;
        }
        if (hit.collider.tag == "Portal")
        {
            float smallExtra = 0.1f; // This is used to help the raycast after the portal
            float remainingLength = pickupDistance - Vector3.Distance(startPos, hit.point) + smallExtra;
            var inPortal = hit.collider.gameObject.transform.parent.GetComponent<Portal>();
            var outPortal = inPortal.linkedPortal;
            Vector3 hitPosIn = inPortal.transform.InverseTransformPoint(hit.point);
            Vector3 hitPosOut = outPortal.transform.TransformPoint(hitPosIn);
            Vector3 startPosOut = outPortal.transform.TransformPoint(inPortal.transform.InverseTransformPoint(startPos));

            if (hitPosIn.z < 0)
            {
                smallExtra = -smallExtra;
            }

            Vector3 directionToPortal = (hitPosOut - startPosOut).normalized;

            SelectItem(hitPosOut + new Vector3(0, 0, smallExtra), directionToPortal, remainingLength);
            return;
        }
        if (hit.collider.tag == "Item")
        {
            SmartOutLine(hit.collider.gameObject);
            PickupItemCheck(hit.collider.gameObject);
            return;
        }
        if (hit.collider.tag == "Interactable")
        {
            SmartOutLine(hit.collider.gameObject);
            if (pickUpButtonClicked)
            {
                hit.collider.gameObject.GetComponent<IInteractable>().Interact();
                pickUpButtonClicked = false;
            }
            return;
        }
        SmartOutLine(null);
    }

    private void PickupItemCheck(GameObject hitGameObject)
    {
        if (pickUpButtonClicked)
        {
            print("Picking up item");
            PickupItem(hitGameObject);
            pickUpButtonClicked = false;
        }
    }

    private bool DropItemCheck()
    {
        if (pickUpButtonClicked)
        {
            print("Dropping item");
            DropItem();
            pickUpButtonClicked = false;
            return true;
        }
        return false;
    }



    private void SmartOutLine(GameObject itemHit)
    {
        if (itemHit == null)
        {
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
            return;
        }
        if (itemHit.GetComponent<Outline>() == null)
        {
            if (currentOutline != null)
            {
                currentOutline.enabled = false;
                currentOutline = null;
            }
            return;
        }
        if (currentOutline == null)
        {
            currentOutline = itemHit.GetComponent<Outline>();
            currentOutline.enabled = true;
            return;
        }
        if (currentOutline.gameObject != itemHit)
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
        heldItem = item;
        heldItemRB = heldItem.GetComponent<Rigidbody>();
        
        rotationObject.rotationObject = item.transform;

        heldItemRB.useGravity = false;
        heldItemRB.isKinematic = false;
        heldItemRB.drag = 20.0f;
        heldItemRB.constraints = RigidbodyConstraints.FreezeRotation;
        heldItem.layer = 2;
    }

    public void DropItem()
    {

        if(heldItem == null){
            return;
        }
        
        if (rotationObject.HasRotatedObject()) { return; }
        
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
        rotationObject.rotationObject = null;
    }


    private void HandleDoorPlacement(Vector3 startPos, Vector3 direction)
    {
        RaycastHit ray = SendRaycast(startPos, direction, 5f);
        if (ray.collider != null && ray.collider.tag == "Floor")
        {
            Quaternion rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            
            //if (Vector3.Distance(Vector3.zero, transform.position) > maxDistanceFromCenter) return;
            doorFrameOutline.position = ray.point + rotation * doorFrameOffset;
            // Set rotation to face the camera
            doorFrameOutline.rotation = rotation;
            
            if (pickUpButtonClicked)
            {
                doorFrameOutline.position = doorFrameOutlineOriginalPosition;
                arObjectPlacer.SpawnDoor();
                DoorSelector.selectedRoomType = RoomType.None;
                pickUpButtonClicked = false;
            }
        }
        else
        {
            doorFrameOutline.position = doorFrameOutlineOriginalPosition;
            doorFrameOutline.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void HandleFloorPlacement()
    {
        
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.FeaturePoint))
        {
            Pose hitPose = hits[0].pose;
            
            // Calculate the y differance between the floor and the camera
            float yDiff = hitPose.position.y - floor.position.y;
            transform.parent.position -= new Vector3(0, yDiff, 0);
        }
        
    }

    public void PickupTouch()
    {
        pickUpButtonClicked = true;
    }
}
