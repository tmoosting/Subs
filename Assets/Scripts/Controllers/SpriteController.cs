using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    public static SpriteController Instance;

    private void Awake()
    {
        Instance = this;
    }


    public Sprite iconDestroyer;
    public Sprite iconDestroyerGreen;
    public Sprite iconDestroyerGreener;
    public Sprite iconMerchant;
    public Sprite iconMerchantGreen;
    public Sprite iconMerchantGreener;
    public Sprite iconUboat;
    public Sprite iconUboatGreen;
    public Sprite iconUboatGreener;

}
