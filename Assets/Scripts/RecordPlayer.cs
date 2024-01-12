using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    private GameObject currentDisc;
    [SerializeField] private bool acceptDiscs = true;
    [SerializeField] private Transform snapPoint;
    [SerializeField] private PickUpAR pickUpAr;
    [SerializeField] private bool callMethod = false;
    [SerializeField] private AudioClip[] audioClipList;
    [SerializeField] private GameObject[] discList;
    [SerializeField] private Safe safe;

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
        if (acceptDiscs && InList(collider.gameObject))
        {
            currentDisc = collider.gameObject;
            Rigidbody rb = currentDisc.GetComponent<Rigidbody>();

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            currentDisc.transform.rotation = Quaternion.identity;

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
                safe.CompleteRecordPuzzle();
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
    
    public void SetAcceptDiscs(bool accept) {
        acceptDiscs = accept;
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
