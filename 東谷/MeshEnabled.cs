using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//hand_right_renderPart_0

public class MeshEnabled : MonoBehaviour
{
    public GameObject targetMesh;
    public GameObject gunModel;

    void Update()
    {
        Component[] meshs = null;
        if (targetMesh.GetComponentInChildren<SkinnedMeshRenderer>())
        {
            meshs = targetMesh.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
        }
        if (meshs != null)
        {
            foreach (SkinnedMeshRenderer mesh in meshs)
            {
                mesh.enabled = false;
            }
            if (gunModel != null)
            {
                gunModel.SetActive(true);
            }
            DestroyImmediate(this);
        }
    }
}
