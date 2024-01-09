using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCog : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 1f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
