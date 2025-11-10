using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFall : MonoBehaviour
{
    public float fallSpeed = 1f;
    public Material mat;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        mat = GetComponent<MeshRenderer>().sharedMaterial;
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisable()
    {
        if (mat != null)
        {
            Vector2 offset = mat.mainTextureOffset;
            offset = new Vector2(0f, 0f);
            mat.mainTextureOffset = offset;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (mat != null)
        {
            Vector2 offset = mat.mainTextureOffset;
            offset.y += Time.deltaTime * fallSpeed;
            mat.mainTextureOffset = offset;
        }
    }
}
