using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCollider : MonoBehaviour
{

    [SerializeField] private Puzzle puzzle;
    [SerializeField] private PuzzleItemShape shape;
    private void OnTriggerEnter(Collider collider)
    {
        PuzzleItem item = collider.gameObject.GetComponent<PuzzleItem>();
        if (item == null) return;

        if (item.shape == shape)
        {
            UpdatePuzzle(shape, true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        PuzzleItem item = collider.gameObject.GetComponent<PuzzleItem>();
        if (item == null) return;

        if (item.shape == shape)
        {
            UpdatePuzzle(shape, false);
        }
    }

    private void UpdatePuzzle(PuzzleItemShape shape, bool status)
    {
        switch (shape) {
            case PuzzleItemShape.Cube:
                puzzle.hasCube = status;
                break;
            case PuzzleItemShape.Cylinder:
                puzzle.hasCylinder = status;
                break;
            case PuzzleItemShape.Triangle:
                puzzle.hasTriangle = status;
                break;
        }
    }
}
