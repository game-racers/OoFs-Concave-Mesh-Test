using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CutoutObject : MonoBehaviour
{
    [SerializeField] Transform targetObj;

    [SerializeField] LayerMask wallMask;

    Camera mainCam;

    [SerializeField] List<Material> cutoutShaderMats = new List<Material>();

    private void Awake()
    {
        mainCam = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector2 cutoutPos = mainCam.WorldToViewportPoint(targetObj.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObj.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        for (int i = 0; i < hitObjects.Length; i++)
        {
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            for (int m = 0; m < materials.Length; m++)
            {
                materials[m].SetVector("_CutoutPos", cutoutPos);
                materials[m].SetFloat("_CutoutSize", 0.1f);
                materials[m].SetFloat("_FalloffSize", .05f);
            }
        }

        //// less raycasts but holes everywhere
        //foreach (Material mat in cutoutShaderMats)
        //{
        //    mat.SetVector("_CutoutPos", cutoutPos);
        //    mat.SetFloat("_CutoutSize", 0.1f);
        //    mat.SetFloat("_FalloffSize", 0.1f);
        //}
    }
}
