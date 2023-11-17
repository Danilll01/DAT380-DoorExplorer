using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class BetterPortalTraveller : PortalTraveller
{
    [SerializeField] private GameObject objectBase;
    [SerializeField] private GameObject baseObj;


    private Vector3 vector = new Vector3(90,0,0);

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        
        transform.position = pos;
        transform.rotation = rot;
        
    }


    public override void EnterPortalThreshold()
    {
        if (objectBase == null)
        {
            objectBase = new GameObject();
        }

        if (graphicsClone == null)
        {
            graphicsClone = Instantiate(graphicsObject, objectBase.transform);

            


            foreach (Renderer renderer in graphicsClone.GetComponents<Renderer>())
            {
                renderer.shadowCastingMode = ShadowCastingMode.Off;
            }

            foreach (Renderer renderer in graphicsClone.GetComponentsInChildren<Renderer>())
            {
                renderer.shadowCastingMode = ShadowCastingMode.Off;
            }

            //graphicsClone.transform.parent = graphicsObject.transform.parent;
            graphicsClone.transform.localScale = graphicsObject.transform.localScale;
            originalMaterials = GetMaterials(graphicsObject);
            cloneMaterials = GetMaterials(graphicsClone);
        }
        else
        {
            graphicsClone.SetActive(true);
        }
    }
}
