using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
     
    // range up to 50 miles

    [SerializeField] private Transform pfRadarPing;
    [SerializeField] private LayerMask radarLayerMask;

    public bool enableSonar;
    private Transform pulseTransform;
    private float range;
    public float rangeMax;
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
        pulseTransform = transform.Find("SonarPulse");
        pulseTransform.gameObject.SetActive(false);
        pulseSpriteRenderer = pulseTransform.GetComponent<SpriteRenderer>();
        pulseColor = pulseSpriteRenderer.color;
        alreadyPingedColliderList = new List<Collider2D>();
    
    }
    private void Start()
    {
        if (enableSonar == true)
            SoundController.Instance.PlaySonarPing();
    }
    private void Update()
    {
        if (enableSonar == true)
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
                Debug.Log("hit: " + raycastHit2D.collider.gameObject.name);

                // Hit something
                if (!alreadyPingedColliderList.Contains(raycastHit2D.collider))
                {
                    alreadyPingedColliderList.Add(raycastHit2D.collider); 
                    Transform radarPingTransform = Instantiate(pfRadarPing, raycastHit2D.collider.transform.position, Quaternion.identity); 
                    
                    RadarPing radarPing = radarPingTransform.GetComponent<RadarPing>();

                    if (raycastHit2D.collider.gameObject.GetComponent<Ship>() != null)
                    {
                        if (GetComponent<Captain>() != null)
                           GetComponent<Captain>().DetectSonar(raycastHit2D.collider.gameObject);
                        radarPing.SetColor(new Color(1, 0.1f, 0.2f));
                    }
                    if (raycastHit2D.collider.gameObject.GetComponent<Pillenwerfer>() != null)
                    {
                        if (GetComponent<Captain>() != null)
                            GetComponent<Captain>().DetectSonar(raycastHit2D.collider.gameObject);
                        radarPing.SetColor(new Color(1, 0.1f, 0.2f));
                    }
                    radarPing.SetDisappearTimer(rangeMax / rangeSpeed * 1f);
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
            range = 0f;
            alreadyPingedColliderList.Clear();
            pulseTransform.localScale = new Vector3(range, range);
            intervalStandby = true;
            if (GetComponent<Captain>() != null)
                GetComponent<Captain>().SonarCycle();
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
        SoundController.Instance.PlaySonarPing();
        intervalStandby = false;

    }
}
