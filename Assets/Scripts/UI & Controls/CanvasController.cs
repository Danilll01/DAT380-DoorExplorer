using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasOverlay : MonoBehaviour
{

    private GameObject landScape;
    private GameObject portrait;
    
    // Start is called before the first frame update
    void Start()
    {
        Screen.autorotateToPortrait = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.orientation = ScreenOrientation.AutoRotation;

        landScape = transform.Find("Landscape").gameObject;
        portrait = transform.Find("Portrait").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            portrait.SetActive(true);
            landScape.SetActive(false);
        }
        else
        {
            portrait.SetActive(false);
            landScape.SetActive(true);
        }
    }
}
