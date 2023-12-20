using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sesame : MonoBehaviour
{

    [SerializeField] private GameObject item;
    [SerializeField] private GameObject spawnItem;

    [Header("What disapears after interaction")]
    [SerializeField] private bool itemDis = false;
    [SerializeField] private bool itemHolderDis = false;
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private float fadeOutTime = 1.0f;

    [Header("Delay between spawns")]
    [SerializeField] private float delay = 3.0f;

    [Header("Trigger collider size multiplier")]
    [SerializeField] private float multiplier = 1.5f;

    private bool coRoutineActive = false;
    private BoxCollider boxCollider;
    private ParticleSystem particleSystem;

    // Start is called before the first frame update
    void Awake()
    {
        if (GetComponent<ParticleSystem>() != null)
        {
            particleSystem = GetComponent<ParticleSystem>();
            particleSystem.Play();
        }
        if (GetComponent<BoxCollider>() != null)
        {
            BoxCollider oldCollider = GetComponent<BoxCollider>();
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = oldCollider.center;
            boxCollider.size = oldCollider.size;
            boxCollider.size *= multiplier;
            boxCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (item == null)
        {
            return;
        }
        if (item == other.gameObject)
        {
            Interaction();
        }
    }

    private void Interaction()
    {
        if (itemDis)
        {
            item.SetActive(false);
        }

        ThingStarts();

        if (itemHolderDis)
        {
            Destroy(gameObject);
        }
        if(fadeOut){
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(FadeOut());
        }
    }

    private void ThingStarts()
    {
        if (coRoutineActive == false)
        {
            StartCoroutine(DelayedAction(delay));
            if (spawnItem != null)
            {

                Instantiate(spawnItem, transform.position, transform.rotation);
                particleSystem.Play();

            }else{
                particleSystem.Play();
            }
        }
    }

    IEnumerator FadeOut(){
        yield return new WaitForSeconds(fadeOutTime);
        Destroy(gameObject);
    }

    IEnumerator DelayedAction(float delayTime)
    {
        coRoutineActive = true;
        yield return new WaitForSeconds(delayTime);
        coRoutineActive = false;
    }

}
