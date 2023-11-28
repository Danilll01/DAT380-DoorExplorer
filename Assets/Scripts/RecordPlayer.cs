using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private Record playingFrom;
    [SerializeField] private Transform snapPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        Record record = collider.GetComponent<Record>();
        if (record != null)
        {
            collider.transform.position = snapPoint.position;
            
            AudioClip audioClip = record.GetAudioClip();
            //print("Playing" + audioClip.name);
            audioSource.clip = audioClip;
            audioSource.Play();
            playingFrom = record;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Record record = collider.GetComponent<Record>();
        if (playingFrom == record)
        {
            audioSource.Stop();
            playingFrom = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (playingFrom != null)
        {
            // Only play if the record is close to the snap point, otherwise pause
            if (Vector3.Distance(playingFrom.transform.position, snapPoint.position) > 0.07f)
            {
                audioSource.Pause();
            }
            else
            {
                playingFrom.transform.position = snapPoint.position;
                audioSource.UnPause();
            }
        }
    }
}
