using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasOverlay : MonoBehaviour
{
    [Header("Landscape")]
    [SerializeField] private GameObject canvasLandscape;
    [Header("Portrait")]
    [SerializeField] private GameObject canvasPortrait;
    // Start is called before the first frame update
    void Start()
    {
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            canvasPortrait.SetActive(true);
            canvasLandscape.SetActive(false);
        }
        else
        {
            canvasPortrait.SetActive(false);
            canvasLandscape.SetActive(true);
        }
    }
}
