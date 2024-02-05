using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatTexture : MonoBehaviour
{
    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.mainTexture.wrapMode = TextureWrapMode.Repeat;
        }
    }
}
