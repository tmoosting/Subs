using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class subLocator : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] Camera cam;

    public float gs;

    // Start is called before the first frame update
    void Start()
    {
        FindHeightMapValue();
    }

    // Update is called once per frame
    void Update()
    {
        FindHeightMapValue();
    }

    void FindHeightMapValue(){
        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        int x = (int) screenPos.x;
        int y = (int) screenPos.y;

        Texture2D tex2d = (Texture2D)image.texture;
        Color c = tex2d.GetPixel(x,y);
        // Calculate greyscale
        // gs = (R + G + B) / 3
        gs = (c.r + c.g + c.b)/3;
    }
}
