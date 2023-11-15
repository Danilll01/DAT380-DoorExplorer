using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicker : MonoBehaviour
{

    [SerializeField] private bool distanceCheck = false;
    [SerializeField] private Camera mainCamera = null;

    void OnMouseDown()
    {
        if (!distanceCheck )
        {
            MoveObjectToCamera();
        }
        else if (Vector3.Distance(transform.position, mainCamera.transform.position) < 20.0f)
        {
            MoveObjectToCamera();
        }
    }

    private void MoveObjectToCamera()
    {
        StopCoroutine("MoveObjectCoroutine");
        transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        StartCoroutine(MoveObjectToCameraCoroutine());
    }

    private void ShrinkItem()
    {
        transform.localScale = transform.localScale * 0.5f;
    }

    private void GrowItem()
    {
        transform.localScale = transform.localScale * 2.0f;
    }

    private IEnumerator MoveObjectToCameraCoroutine()
    {
        Transform cameraTransform = mainCamera.transform;
        Vector3 start = transform.position;
        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 pointInFront = cameraTransform.position + cameraTransform.forward * 2.0f;
            transform.position = Vector3.Lerp(start, pointInFront, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = cameraTransform.position + cameraTransform.forward * 2.0f;
    }
}
