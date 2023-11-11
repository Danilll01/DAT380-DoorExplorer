using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAble : MonoBehaviour
{

    private Material material;

    private void Start()
    {
        Material[] materials = GetComponent<Renderer>().materials;
        foreach (Material mat in materials)
        {
            Debug.Log(mat.name);
            if (mat.name == "PickAbleShader (Instance)")
            {
                material = mat;
            }
        }
        SetVisibility(false);
    }

    private void OnMouseEnter()
    {

        SetVisibility(true);
    }

    private void OnMouseExit()
    {

        SetVisibility(false);
    }

    private void SetVisibility(bool isVisible)
    {
        material.color = isVisible ? Color.white : Color.clear;
    }
}
