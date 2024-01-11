using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Remote : MonoBehaviour, IInteractable
{

    [SerializeField] private GameObject videoGameObject;
    [SerializeField] private GameObject pictureGameObject;
    [SerializeField] private VideoClip[] videoClips;
    [SerializeField] private Material[] materials;
    private VideoPlayer videoPlayer;
    private int currentMaterial = 0; 
    private int currentClip = 0;
    private bool onlyPictures = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (videoGameObject != null)
        {
            videoPlayer = videoGameObject.GetComponent<VideoPlayer>();
        }
        else
        {
            onlyPictures = true;
        }

    }

    private void SwapVideo()
    {
        if (currentClip == videoClips.Length - 1)
        {
            videoPlayer.clip = videoClips[currentClip];
            currentClip = 0;
        }
        else
        {
            videoPlayer.clip = videoClips[currentClip];
            currentClip++;
        }
    }

    private void SwapMaterial()
    {
        if (currentMaterial == materials.Length - 1)
        {
            pictureGameObject.GetComponent<MeshRenderer>().material = materials[currentMaterial];
            currentMaterial = 0;
        }
        else
        {
            pictureGameObject.GetComponent<MeshRenderer>().material = materials[currentMaterial];
            currentMaterial++;
        }
    }

    private void HandleVideo()
    {
        if (pictureGameObject.activeSelf)
        {
            pictureGameObject.SetActive(false);
            videoGameObject.SetActive(true);
            SwapVideo();
        }
        else
        {
            SwapVideo();
        }
    }

    private void HandlePicture()
    {
        if(pictureGameObject.GetComponent<MeshRenderer>() != null)
        {
            SwapMaterial();
        }
    }

    public void Interact()
    {
        if (onlyPictures)
        {
            HandlePicture();
        }
        else
        {
            HandleVideo();
        }
    }
}
