using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{

    public bool hasCube = false;
    public bool hasTriangle = false;
    public bool hasCylinder = false;

    [SerializeField] private ParticleSystem particleSystem;

    
    void Update()
    {
        // If all pieces are in the puzzle, spawn particles
        if (hasCube && hasTriangle && hasCylinder)
        {
            if (particleSystem != null && !particleSystem.isPlaying)
            particleSystem.Play();
        }
    }
}
