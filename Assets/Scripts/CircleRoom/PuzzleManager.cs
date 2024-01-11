using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private float startHeight;
    [SerializeField] private float endHeight;
    [SerializeField] private float animationTime;
    
    // Start is called before the first frame update
    void Start()
    {
        startHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (Math.Abs(transform.localRotation.eulerAngles.y - (-180)) < 0.001)
            {
                StartCoroutine(OutAnimation());
            }
            
            if (Math.Abs(transform.localRotation.eulerAngles.y - (0)) < 0.001)
            {
                StartCoroutine(InAnimation());
            }
        } 
    }

    private IEnumerator OutAnimation()
    {   
        float timer = 0;
        while (timer < animationTime)
        {
            transform.localRotation = Quaternion.Euler(0, Mathf.SmoothStep(-180, 0, timer / animationTime), 0);
            
            //transform.position = new Vector3(transform.position.x, Mathf.SmoothStep())
            yield return null;
        }
    }
    
    private IEnumerator InAnimation()
    {
        float timer = 0;
        while (timer < animationTime)
        {
            transform.localRotation = Quaternion.Euler(0, Mathf.SmoothStep(0, -180, timer / animationTime), 0);
            
            //transform.position = new Vector3(transform.position.x, Mathf.SmoothStep())
            yield return null;
        }
    }
}
