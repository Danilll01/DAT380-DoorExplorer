using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private float startHeight;
    [SerializeField] private float endHeight;
    [SerializeField] private float animationTime = 7f;
    [SerializeField] [Range(0,1)] private float animationChange = 0.8f;
    
    // Start is called before the first frame update
    void Start()
    {
        startHeight = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (Math.Abs(transform.localRotation.eulerAngles.y - (180)) < 0.1)
            {
                StartCoroutine(OutAnimation());
            }
            
            if (Math.Abs(transform.localRotation.eulerAngles.y) < 0.1 || Math.Abs(transform.localRotation.eulerAngles.y - (360)) < 0.1)
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

            float laterAnimationLerp = Mathf.InverseLerp(animationChange * animationTime, animationTime, timer);
            //print("InverseLerp: " + laterAnimationLerp + "time: " + timer);
            
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.SmoothStep(startHeight, endHeight, laterAnimationLerp), transform.localPosition.z);
            //print("ANIMATION: " + Mathf.SmoothStep(startHeight, endHeight, laterAnimationLerp));
            timer += Time.deltaTime;
            yield return null;
        }
    }
    
    private IEnumerator InAnimation()
    {
        float timer = 0;
        while (timer < animationTime)
        {
            transform.localRotation = Quaternion.Euler(0, Mathf.SmoothStep(0, -180, timer / animationTime), 0);

            float laterAnimationLerp = Mathf.InverseLerp(0, animationTime - (animationChange * animationTime), timer);
            //print("InverseLerp: " + laterAnimationLerp + "time: " + timer);
            
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.SmoothStep(endHeight, startHeight, laterAnimationLerp), transform.localPosition.z);
            //print("ANIMATION: " + Mathf.SmoothStep(startHeight, endHeight, laterAnimationLerp));
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
