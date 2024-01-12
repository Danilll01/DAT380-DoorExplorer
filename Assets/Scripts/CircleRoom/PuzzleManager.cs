using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private float startHeight;
    [SerializeField] private float endHeight;
    [SerializeField] private float animationTime = 7f;
    [SerializeField] [Range(0,1)] private float animationChange = 0.8f;

    [SerializeField] private Transform winPeg;

    [SerializeField] private MechanicalHinge hinge1;
    [SerializeField] private MechanicalHinge hinge2;
    
    private bool Button1Pressed = false;
    private bool Button2Pressed = false;
    
    [SerializeField] Transform peg1;
    [SerializeField] Transform peg2;
    private Quaternion lastPeg1Rotation;
    private Quaternion lastPeg2Rotation;
    
    // Start is called before the first frame update
    void Start()
    {
        startHeight = transform.localPosition.y;
        lastPeg1Rotation = peg1.rotation;
        lastPeg2Rotation = peg2.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Math.Abs(transform.localRotation.eulerAngles.y - (180)) < 0.1)
        {
            if (peg1.rotation != lastPeg1Rotation && peg2.rotation != lastPeg2Rotation && winPeg.childCount == 1)
            {
                hinge1.OnlyOpen();
                hinge2.OnlyOpen();
                print("ButtonPressed: " + Button1Pressed);
                if (Button1Pressed && Button2Pressed)
                {
                    print("WIN!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    StartCoroutine(OutAnimation());
                }
            }
            else
            {
                hinge1.OnlyClose();
                hinge2.OnlyClose();
            }
        }
        
        if (Math.Abs(transform.localRotation.eulerAngles.y) < 0.1 || Math.Abs(transform.localRotation.eulerAngles.y - (360)) < 0.1)
        {
            if (winPeg.childCount == 0)
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

    public void PressButton(int buttonNumber)
    {
        switch (buttonNumber)
        {
            case 1:
                Button1Pressed = true;
                break;
            case 2:
                Button2Pressed = true;
                break;
            default:
                Debug.LogError("Button number not found");
                break;
        }
    }
}
