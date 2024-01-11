using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageDoor : MonoBehaviour
{
    private float startRotation;
    [SerializeField] private float endRotation = 90f;
    private float startPosition;
    [SerializeField] private float endPosition = 1f;
    [SerializeField] private float animationTime = 1f;
    [SerializeField] private float animationChange = 0.5f;
    
    private void Start()
    {
        startRotation = transform.localRotation.eulerAngles.x;
        startPosition = transform.localPosition.z;
    }
    
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T)) StartCoroutine(OpenDoor());
    }
    
    private IEnumerator OpenDoor()
    {   
        float timer = 0;
        while (timer < animationTime)
        {
            transform.localRotation = Quaternion.Euler(Mathf.SmoothStep(startRotation, endRotation, timer / animationTime), 0, 0);

            float laterAnimationLerp = Mathf.InverseLerp(animationChange * animationTime, animationTime, timer);
            //print("InverseLerp: " + laterAnimationLerp + "time: " + timer);
            
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.SmoothStep(startPosition, endPosition, laterAnimationLerp));
            //print("ANIMATION: " + Mathf.SmoothStep(startHeight, endHeight, laterAnimationLerp));
            timer += Time.deltaTime;
            yield return null;
        }
    }
    
    public void Open()
    {
        StartCoroutine(OpenDoor());
    }
}
