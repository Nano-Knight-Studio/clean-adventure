using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    public Material material;
    public Vector2 scroll;
    Vector2 currentScroll;

    void Update ()
    {
        currentScroll += scroll * Time.deltaTime;
        material.SetTextureOffset("_BaseMap", currentScroll);
    }
}
