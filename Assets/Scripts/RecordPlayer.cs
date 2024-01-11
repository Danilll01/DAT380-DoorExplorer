using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private GameObject currentDisc;
    [SerializeField] private Transform snapPoint;
    [SerializeField] private PickUpAR pickUpAr;
    [SerializeField] private bool callMethod = false;
    [SerializeField] private AudioClip[] audioClipList;
    [SerializeField] private GameObject[] discList;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (currentDisc != null)
        {
            if (Vector3.Distance(currentDisc.transform.position, snapPoint.position) > 0.07f)
            {
                RecordRemoved();
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (InList(collider.gameObject))
        {
            currentDisc = collider.gameObject;
            pickUpAr.DropItem();
            currentDisc.transform.position = snapPoint.position;

            int index = IndexOfDisc(currentDisc);

            if (audioClipList[index] != null)
            {
                audioSource.clip = audioClipList[index];
                audioSource.Play();
            }

            if (callMethod)
            {
                //
            }
        }
    }

    private bool InList(GameObject check)
    {
        for (int i = 0; i < discList.Length; i++)
        {
            if (discList[i] == check)
            {
                return true;
            }
        }
        return false;
    }

    private int IndexOfDisc(GameObject check)
    {
        for (int i = 0; i < discList.Length; i++)
        {
            if (discList[i] == check)
            {
                return i;
            }
        }
        return -1;
    }

    private void RecordRemoved()
    {
        audioSource.Pause();
        currentDisc = null;
    }

    /* private void OnTriggerExit(Collider collider)
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
    } */
}
