using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Safe : MonoBehaviour
{
    [SerializeField] private bool ballPuzzleComplete = false;
    [SerializeField] private bool recordPuzzleComplete = false;
    private bool puzzleComplete = false;


    private void Update()
    {
        if (!puzzleComplete && ballPuzzleComplete && recordPuzzleComplete)
        {
            GetComponent<MechanicalHinge>().Open();
            puzzleComplete = true;
        }
    }

    public void CompleteBallPuzzle()
    {
        ballPuzzleComplete = true;
    }
    
    public void CompleteRecordPuzzle()
    {
        recordPuzzleComplete = true;
    }
}
