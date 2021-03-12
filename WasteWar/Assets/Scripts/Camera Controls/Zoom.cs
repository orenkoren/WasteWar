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

    void Start()
    {
        GetBezierCurvePointPositions();
        AdjustCameraZoom(true, CameraConstants.Instance.INCREMENT_T);
    }

    void Update()
    {
        GetBezierCurvePointPositions();
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (t < CameraConstants.Instance.MAX_ZOOM_IN)
            {
                AdjustCameraZoom(false, CameraConstants.Instance.INCREMENT_T);
            }

        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (t > CameraConstants.Instance.MIN_ZOOM_IN)
            {
                AdjustCameraZoom(false, CameraConstants.Instance.DECREMENT_T);
            }
        }

    }

    private void GetBezierCurvePointPositions()
    {
        p1 = route.GetChild(0).position;
        p2 = route.GetChild(1).position;
        p3 = route.GetChild(2).position;
        p4 = route.GetChild(3).position;
    }

    private void AdjustCameraZoom(bool isFirstCall, bool isToBeIncremented)
    {
        if (isFirstCall)
            while (cam.rotation.eulerAngles.x > CameraConstants.Instance.INITIAL_ZOOM_ANGLE_X_AXIS)
            {
                ZoomInstructionSequence(isToBeIncremented);

            }
        else
        {
            ZoomInstructionSequence(isToBeIncremented);
        }
    }

    private void AdjustParameter(bool isToBeIncremented)
    {
        if (isToBeIncremented)
            t += Time.deltaTime * CameraConstants.Instance.ZOOM_SPEED;
        else
            t -= Time.deltaTime * CameraConstants.Instance.ZOOM_SPEED;
       // t = t * t * t * (t * (6f * t - 15f) + 10f);
    }
    private void ZoomInstructionSequence(bool isToBeIncremented)
    {
        AdjustParameter(isToBeIncremented);
        cam.rotation = MathUtilBasic.CalcRotationChangeAlongTheCurve(t, rotationRoute.GetChild(0), rotationRoute.GetChild(1));
        cam.position = MathUtilBasic.CalcCurrPosAlongTheCurve(t, p1, p2, p3, p4);
    }
}