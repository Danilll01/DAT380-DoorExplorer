using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTest : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 vec = new Vector3(0, 1.7f, 8f);
    private bool teleported = false;
    private Transform camera;

    private void Start()
    {
        camera = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(camera.position, Vector3.up, 10 * Time.deltaTime);
    }
}
