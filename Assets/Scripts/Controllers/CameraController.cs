using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;


    public CinemachineVirtualCamera followCam;


    Vector3 lastFramePosition;
    public bool followMode;

    public float maxZoomIn = 4f;
    public float maxZoomOut = 320f;
    public bool automaticStrategyView;
    public float strategyHeight;

    private void Awake()
    {
        Instance = this;
        followCam.gameObject.SetActive(false);

    }
    void Update()
    {
        // HANDlE PANNING
        Vector3 currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        if (Input.GetMouseButton(1) )
        {
            Vector3 diff = lastFramePosition - currFramePosition;
            Camera.main.transform.Translate(diff);
        }
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;

        //HANDLE ZOOM
       
        if (followMode == true)
        {
            followCam.m_Lens.OrthographicSize -= followCam.m_Lens.OrthographicSize * Input.GetAxis("Mouse ScrollWheel");
            followCam.m_Lens.OrthographicSize = Mathf.Clamp(followCam.m_Lens.OrthographicSize, maxZoomIn, maxZoomOut); 
        }
        else
        {
            Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
           Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, maxZoomIn, maxZoomOut);
        }
      

        // FOLLOW SHIP
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (followMode == false)
            {
                CameraController.Instance.ZoomToShip(GameController.Instance.selectedShip);  
            }

            else
            {
                followMode = false;
                followCam.gameObject.SetActive(false);
                Camera.main.orthographicSize = followCam.m_Lens.OrthographicSize;
                UIController.Instance.followIcon.SetActive(false);
            }
        }
        // ZOOM TO SHIP
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    Camera.main.orthographicSize = 5;
        //    followCam.m_Lens.OrthographicSize = 5;
        //    if (followMode == false)
        //    {
        //       ZoomToShip(GameController.Instance.selectedShip);
        //    }
        //}

        // Check for strategy view
        if (automaticStrategyView == true)
        {
            float cameraHeight;
            if (followMode == true)
                cameraHeight = followCam.m_Lens.OrthographicSize;
            else
                cameraHeight = Camera.main.orthographicSize;
            if (cameraHeight > strategyHeight)
                UIController.Instance.strategyViewEnabled = true;
            else
                UIController.Instance.strategyViewEnabled = false;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                UIController.Instance.strategyViewEnabled = !UIController.Instance.strategyViewEnabled;
            }
        }
       

    }


    public void ShowStrategyOverlay()
    {

    }

    public void ZoomToShip (Ship ship)
    {
        followCam.m_Lens.OrthographicSize = Camera.main.orthographicSize;
        followMode = true;
        followCam.Follow = ship.transform; 
        followCam.gameObject.SetActive(true);
        UIController.Instance.followIcon.SetActive(true);
    }
}
