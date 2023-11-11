using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAble : MonoBehaviour
{

    private Outline script;

    void Start()
    {
        script = GetComponent<Outline>();
        if (script != null){
            script.enabled = false;
        }
    }

}
