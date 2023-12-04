using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTest : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 vec = new Vector3(0, 1.7f, 8f);
    private bool teleported = false;

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase.Equals(TouchPhase.Began))
        {
            print("Touch");
            transform.position = teleported ? originalPos : vec;
            teleported = !teleported;
        }
    }
}
