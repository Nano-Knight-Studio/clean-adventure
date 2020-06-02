using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    public Material material;
    public Vector2 scroll;
    public float acceleration;
    Vector2 currentScroll;
    public bool dependsOnCommand = false;

    void Update ()
    {
        currentScroll = Vector2.Lerp(currentScroll, scroll, acceleration * Time.deltaTime);
        material.mainTextureOffset += currentScroll * Time.deltaTime;
    }
}
