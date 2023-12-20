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

    [Header("Delay between spawns")]
    [SerializeField] private float delay = 3.0f;

    private bool coRoutineActive = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(item == null){
            return;
        }
        if(Vector3.Distance(item.transform.position, transform.position) < 1.0f){
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
    }

    private void ThingStarts(){
        if(coRoutineActive == false){
            StartCoroutine(DelayedAction(delay));
            if(spawnItem != null){

                Instantiate(spawnItem, transform.position, transform.rotation);

            }else{
                Debug.Log("No item to spawn");
            }
        }
    }

    IEnumerator DelayedAction(float delayTime)
    {
        coRoutineActive = true;
        yield return new WaitForSeconds(delayTime);
        coRoutineActive = false;
    }

}
