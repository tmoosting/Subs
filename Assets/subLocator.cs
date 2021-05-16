using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class subLocator : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] Camera cam;

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

        RectTransform rt = image.rectTransform;
        float width = rt.rect.width;
        float height = rt.rect.height;

        Texture2D tex2d = (Texture2D)image.texture;

        Color c = tex2d.GetPixel(x,y);
        float gs = c.b;

        Debug.Log("x is " + x);
        Debug.Log("y is " + y);
        Debug.Log("gs is " + gs);
        
        //Debug.Log("Value is " + mapValue);
        //Debug.Log()
    }
}
