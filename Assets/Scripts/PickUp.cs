using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private GameObject selectedObject;
    private bool itemCanFollow = false;
    private Vector3 initialForward;

    void Update()
    {
        RightClick();
        if (selectedObject != null && itemCanFollow)
        {
            CarryItem();
        }
    }

    private void CarryItem()
    {
        selectedObject.transform.position = transform.position + transform.forward * 2.0f;

        RotateItem();
    }

    private void RotateItem()
    {
        Vector3 directionToCamera = transform.position - selectedObject.transform.position;
        Quaternion targetRotation = Quaternion.FromToRotation(initialForward, directionToCamera);
        selectedObject.transform.rotation = targetRotation;
    }

    private void RightClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                PickUpItem();
            }
            else
            {
                DropItem();
            }

        }
    }

    private void PickUpItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit))
        {

            PickAble pickAble = hit.collider.GetComponent<PickAble>();

            if (pickAble != null)
            {
                selectedObject = hit.collider.gameObject;
                selectedObject.GetComponent<Rigidbody>().useGravity = false;
                Debug.Log("Selected Object: " + selectedObject.name);
                MoveObjectToCamera();
            }
        }
    }

    private void DropItem()
    {
        selectedObject.GetComponent<Rigidbody>().useGravity = true;
        itemCanFollow = false;
        selectedObject = null;
    }

    private void MoveObjectToCamera()
    {
        StopCoroutine("MoveObjectCoroutine");
        Rigidbody rb = selectedObject.GetComponent<Rigidbody>();
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        StartCoroutine(MoveObjectToCameraCoroutine());
    }

    private IEnumerator MoveObjectToCameraCoroutine()
    {
        Vector3 start = selectedObject.transform.position;
        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 pointInFront = transform.position + transform.forward * 2.0f;
            selectedObject.transform.position = Vector3.Lerp(start, pointInFront, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = transform.position + transform.forward * 2.0f;
        itemCanFollow = true;
        initialForward = selectedObject.transform.forward;
    }







    private void ShrinkItem()
    {
        selectedObject.transform.localScale = selectedObject.transform.localScale * 0.5f;
    }

    private void GrowItem()
    {
        selectedObject.transform.localScale = selectedObject.transform.localScale * 2.0f;
    }


}
