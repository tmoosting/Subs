using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lookout : MonoBehaviour
{
    // in standard atmospheric conditions, for an observer with eye level above sea level by 1.70 metres (5 ft 7 in), the horizon is at a distance of about 5 kilometres (3.1 mi).

    // pulse expands quickly then hangs around long, slowly fades in and out
    // add a chance-based spot relative to distance? 

    [SerializeField] private Transform pfRadarPing;
    [SerializeField] private LayerMask radarLayerMask;

    public bool enableLookout;
    private Transform pulseTransform;
    private float range;
    public float rangeMax;
    public float rangeResetFactor;
    public float rangeSpeed;
    public float fadeRange;
    public float interval;
    private SpriteRenderer pulseSpriteRenderer;
    private Color pulseColor;
    private List<Collider2D> alreadyPingedColliderList;
    bool intervalStandby;

    [HideInInspector] public List<Uboat> spottedUboats;

    private void Awake()
    {
        pulseTransform = transform.Find("LookoutPulse");
        pulseTransform.gameObject.SetActive(false);
        pulseSpriteRenderer = pulseTransform.GetComponent<SpriteRenderer>();
        pulseColor = pulseSpriteRenderer.color;
        alreadyPingedColliderList = new List<Collider2D>();
        range = rangeMax;

    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if (enableLookout == true)
        {
            if (intervalStandby == false)
            {
                pulseTransform.gameObject.SetActive(true);
                Scan();
                FadePulse();
                ScalePulse();
            }
        }
        else
        {
            pulseTransform.gameObject.SetActive(false);
        }

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
                    

                    if (raycastHit2D.collider.gameObject.GetComponent<Ship>() != null)
                    {
                        if (raycastHit2D.collider.gameObject.GetComponent<Ship>() != GetComponent<Ship>())
                        {
                            if (raycastHit2D.collider.gameObject.GetComponent<Ship>().shipType == Ship.ShipType.UBOAT)
                            {
                                //Transform radarPingTransform = Instantiate(pfRadarPing, raycastHit2D.collider.transform.position, Quaternion.identity);

                                //RadarPing radarPing = radarPingTransform.GetComponent<RadarPing>(); 
                                //radarPing.SetColor(new Color(0.1f, 0.1f, 0.9f));
                                //radarPing.SetDisappearTimer(rangeMax / rangeSpeed * 1f);
                            }
                            if (GetComponent<Captain>() != null)
                                GetComponent<Captain>().DetectLookout(raycastHit2D.collider.gameObject.GetComponent<Ship>());
                    

                        }
                    }
                    if (raycastHit2D.collider.gameObject.GetComponent<Torpedo>() != null)
                    {
                        if (GetComponent<Captain>() != null)
                            GetComponent<Captain>().SightATorpedo(raycastHit2D.collider.gameObject.GetComponent<Torpedo>());

                        //Transform radarPingTransform = Instantiate(pfRadarPing, raycastHit2D.collider.transform.position, Quaternion.identity);

                        //RadarPing radarPing = radarPingTransform.GetComponent<RadarPing>();
                        //radarPing.SetDisappearTimer(rangeMax / rangeSpeed * 1f);


                    }
                }
            }
        }
    }
    void FadePulse()
    {
        if (range > rangeMax - fadeRange)
            pulseColor.a = Mathf.Lerp(0f, 1f, (rangeMax - range) / fadeRange);
        else
            pulseColor.a = 1f;
        pulseSpriteRenderer.color = pulseColor;
    }

    void ScalePulse()
    {
        range += rangeSpeed * Time.deltaTime;
        if (range > rangeMax)
        {
            range = range* rangeResetFactor;
            alreadyPingedColliderList.Clear();
            pulseTransform.localScale = new Vector3(range, range);
            intervalStandby = true;
            if (GetComponent<Captain>() != null)
                GetComponent<Captain>().LookoutCycle();
            StartCoroutine(ScanInterval(interval));
        }
        pulseTransform.localScale = new Vector3(range, range);
    }

    IEnumerator ScanInterval(float interval)
    {
        yield return new WaitForSeconds(interval);
        NewScanCycle();
    }

    void NewScanCycle()
    { 
        intervalStandby = false; 
    }
}
