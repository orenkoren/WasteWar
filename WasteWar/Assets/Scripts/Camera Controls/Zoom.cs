using UnityEngine;

public class Zoom : MonoBehaviour
{
    [SerializeField]
    private Transform route;
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private Transform rotationRoute;

    //parameter for parametric equation
    public float t = 0;

    //points along Bezier curve for smooth movement of camera
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;
    private Vector3 p4;


    private void GetBezierCurvePointPositions()
    {
        p1 = route.GetChild(0).position;
        p2 = route.GetChild(1).position;
        p3 = route.GetChild(2).position;
        p4 = route.GetChild(3).position;
    }

    private void SetCameraZoomLevel(bool initial,bool increment)
    {
        if(initial)
        while (cam.rotation.eulerAngles.x > 60)
        {
            IncreaseParameter(increment);
            cam.rotation = CalculateCurrentRotationBasedOnParameterT();
            cam.position = CalculateCurrentPosBasedOnParameterT();

        }
        else
        {
            IncreaseParameter(increment);
            cam.rotation = CalculateCurrentRotationBasedOnParameterT();
            cam.position = CalculateCurrentPosBasedOnParameterT();
        }

    }

    private void IncreaseParameter(bool increment)
    {
        if (increment)
            t += Time.deltaTime * CameraConstants.ZOOM_SPEED;
        else
            t -= Time.deltaTime * CameraConstants.ZOOM_SPEED;

    }

    private Quaternion CalculateCurrentRotationBasedOnParameterT()
    {
        return Quaternion.Lerp(
                   rotationRoute.GetChild(0).rotation,
                   rotationRoute.GetChild(1).rotation,
                   t);
    }
    //using the parameter and the given curve to specify where the gameObject will be positioned
    private Vector3 CalculateCurrentPosBasedOnParameterT()
    {
        return Mathf.Pow(1 - t, 3) * p1 +
            3 * Mathf.Pow(1 - t, 2) * t * p2 +
            3 * (1 - t) * Mathf.Pow(t, 2) * p3 +
            Mathf.Pow(t, 3) * p4;
    }
   
    void Start()
    {
        GetBezierCurvePointPositions();
        SetCameraZoomLevel(true,true);
    }

    void Update()
    {
        GetBezierCurvePointPositions();
        Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (t < CameraConstants.MAX_ZOOM_IN)
                SetCameraZoomLevel(false, true);
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (t > CameraConstants.MIN_ZOOM_IN)
                SetCameraZoomLevel(false, false);
        }
    }
}
