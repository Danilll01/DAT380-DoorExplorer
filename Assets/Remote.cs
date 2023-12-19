using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Remote : MonoBehaviour, IInteractable
{

    [SerializeField] private GameObject videoGameObject;
    [SerializeField] private GameObject pictureGameObject;
    [SerializeField] private VideoClip[] videoClips;
    private VideoPlayer videoPlayer;
    private int currentClip = 0;

    // Start is called before the first frame update
    void Awake()
    {
        videoPlayer = videoGameObject.GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SwapVideo(){
        if(currentClip == videoClips.Length - 1){
            videoPlayer.clip = videoClips[currentClip];
            currentClip = 0;
        }else{
            videoPlayer.clip = videoClips[currentClip];
            Debug.Log("Current Clip: " + currentClip);
            currentClip++;
        }
    }

    public void Interact()
    {
        if(pictureGameObject.activeSelf){
            pictureGameObject.SetActive(false);
            videoGameObject.SetActive(true);
            SwapVideo();
        }else{
            SwapVideo();
        }
    }
}
