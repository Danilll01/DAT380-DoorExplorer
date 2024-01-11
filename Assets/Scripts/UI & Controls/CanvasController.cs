using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;

public class CanvasOverlay : MonoBehaviour
{

    private bool isLandscape;
    private GameObject landScape;
    private GameObject portrait;
    private GameObject settingsL;
    private GameObject settingsP;
    private GameObject tutorialL;
    private GameObject tutorialP;


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
        settingsL = landScape.transform.Find("SettingsMenu").gameObject;
        settingsP = portrait.transform.Find("SettingsMenu").gameObject;
        tutorialL = landScape.transform.Find("Tutorial").gameObject;
        tutorialP = portrait.transform.Find("Tutorial").gameObject;

        settingsL.SetActive(false);
        settingsP.SetActive(false);

        tutorialL.SetActive(true);
        tutorialP.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            SetChildrenActive(portrait);
            foreach (Transform child in landScape.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        else
        {
            SetChildrenActive(landScape);
            foreach (Transform child in portrait.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private void SetChildrenActive(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if(child == settingsL.transform || child == settingsP.transform)
            {
                if(settingsL.activeSelf || settingsP.activeSelf)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
            else if(child == tutorialL.transform || child == tutorialP.transform)
            {
                if(tutorialL.activeSelf || tutorialP.activeSelf)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
            else
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void CloseSettings()
    {
        settingsL.SetActive(false);
        settingsP.SetActive(false);
    }

    public void OpenSettings()
    {
        if(isLandscape)
        {
            settingsL.SetActive(true);
        }
        else
        {
            settingsP.SetActive(true);
        }
    }

    public void OpenTutorial()
    {
        if(isLandscape)
        {
            settingsL.SetActive(false);
            tutorialL.SetActive(true);
        }
        else
        {
            settingsP.SetActive(false);
            tutorialP.SetActive(true);
        }
    }

    public void CloseTutorial()
    {
        tutorialL.SetActive(false);
        tutorialP.SetActive(false);
    }

    public void ResetGame()
    {
        XRGeneralSettings.Instance.Manager.activeLoader.Stop();
        XRGeneralSettings.Instance.Manager.activeLoader.Deinitialize();
        XRGeneralSettings.Instance.Manager.activeLoader.Initialize();
        XRGeneralSettings.Instance.Manager.activeLoader.Start();
        
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
