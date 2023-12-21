using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GearScript : MonoBehaviour
{
    [SerializeField] private int turnForce = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetTurnForce()
    {
        return turnForce;
    }
}
