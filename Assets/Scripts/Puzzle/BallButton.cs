using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallButton : MonoBehaviour
{
    private bool animationIsPlaying = false;
    [SerializeField] private float animationTimer = 0f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float animationDistance = 0.1f;
    [SerializeField] private Safe safe;
    
    private void Update()
    {
        if (animationIsPlaying)
        {
            float yPos = Mathf.Lerp(0, -animationDistance, animationTimer / animationDuration);
            transform.localPosition = new Vector3(0,yPos, 0);
            if (animationTimer < animationDuration)
            {
                animationTimer += Time.deltaTime;
            }
            else
            {
                safe.CompleteBallPuzzle();
                animationIsPlaying = false;
                animationTimer = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        animationIsPlaying = true;
    }
}
