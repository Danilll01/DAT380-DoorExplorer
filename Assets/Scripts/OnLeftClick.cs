using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clicker : MonoBehaviour
{

    [SerializeField] private bool distanceCheck = false;
    [SerializeField] private Camera mainCamera = null;

    void OnMouseDown(){
    if (!distanceCheck){
        ChangeColor();
    }
    else if (DistanceCamera()){
        ChangeColor();
    }
}

    private void ChangeColor()
    {
        Renderer renderer = transform.GetComponent<Renderer>();
        if (renderer.material.color == Color.red){
            renderer.material.color = Color.blue;
        }
        else{
            renderer.material.color = Color.red;
        }
    }

    private bool DistanceCamera()
    {
        float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
        return distanceToCamera < 20.0f;
    }
}
