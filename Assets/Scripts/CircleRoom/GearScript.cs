using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GearScript : MonoBehaviour
{
    [SerializeField] private int turnForce = 1;

    public int GetTurnForce()
    {
        return turnForce;
    }
}
