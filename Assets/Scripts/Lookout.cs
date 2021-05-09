using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookout : MonoBehaviour
{
    

    // pulse expands quickly then hangs around long, slowly fades in and out


    [SerializeField] private Transform pfRadarPing;
    [SerializeField] private LayerMask radarLayerMask;

    public bool enableLookout;
    private Transform pulseTransform;
    private float range;
    public float rangeMax;
    public float rangeSpeed;
    public float fadeRange;
    private SpriteRenderer pulseSpriteRenderer;
    private Color pulseColor;
    private List<Collider2D> alreadyPingedColliderList;



    private void Awake()
    {
        pulseTransform = transform.Find("LookoutPulse");
        pulseTransform.gameObject.SetActive(false);
        pulseSpriteRenderer = pulseTransform.GetComponent<SpriteRenderer>();
        pulseColor = pulseSpriteRenderer.color;  
        alreadyPingedColliderList = new List<Collider2D>();
    }
    private void Update()
    {
        if (enableLookout == true)
        {
            ScalePulse();
            Scan();
            FadePulse();
        } 
        else
            pulseTransform.gameObject.SetActive(false);
    }

    void ScalePulse()
    {
        range += rangeSpeed * Time.deltaTime;
        if (range > rangeMax) 
            range = 0f;
            alreadyPingedColliderList.Clear(); 
        pulseTransform.localScale = new Vector3(range, range);
    }
    void FadePulse()
    { 
        if (range > rangeMax - fadeRange) 
            pulseColor.a = Mathf.Lerp(0f, 1f, (rangeMax - range) / fadeRange); 
        else 
            pulseColor.a = 1f; 
        pulseSpriteRenderer.color = pulseColor;
    }
    void Scan()
    {
        RaycastHit2D[] raycastHit2DArray = Physics2D.CircleCastAll(transform.position, range / 2f, Vector2.zero, 0f, radarLayerMask);
        foreach (RaycastHit2D raycastHit2D in raycastHit2DArray)
        {
            if (raycastHit2D.collider != null)
            {
                // Hit something
                if (!alreadyPingedColliderList.Contains(raycastHit2D.collider))
                {
                    alreadyPingedColliderList.Add(raycastHit2D.collider);
                    //CMDebug.TextPopup("Ping!", raycastHit2D.point);
                    Transform radarPingTransform = Instantiate(pfRadarPing, raycastHit2D.point, Quaternion.identity);
                    RadarPing radarPing = radarPingTransform.GetComponent<RadarPing>();

                    if (raycastHit2D.collider.gameObject.GetComponent<Ship>() != null)
                    { 
                        GetComponent<Captain>().SightAShip(raycastHit2D.collider.gameObject.GetComponent<Ship>());  
                        radarPing.SetColor(new Color(0, .6f, 1));
                    }
                    else if (raycastHit2D.collider.gameObject.GetComponent<Torpedo>() != null)
                    {
                        GetComponent<Captain>().SightATorpedo(raycastHit2D.collider.gameObject.GetComponent<Torpedo>());
                        radarPing.SetColor(new Color(1, 0, 0));
                    }
                    radarPing.SetDisappearTimer(rangeMax / rangeSpeed * 1.5f);
                }
            }
        }
    }



}
