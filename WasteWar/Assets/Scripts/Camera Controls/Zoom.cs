using UnityEngine;

public class Zoom : MonoBehaviour
{
    private const string TILT = "tilt";
    private const string TRANSLATION = "translation";

    [SerializeField]
    private Transform route;
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private Transform rotationRoute;

    //parameters for parametric equations
    public float tRot = 0;
    public float tPos = 0;

    //points along Bezier curve for smooth movement of camera
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;
    private Vector3 p4;

    private int zoomInTicks = 10;
    private int zoomOutTicks = 10;

    void Start()
    {
        GetBezierCurvePointPositions();
        InitializeCameraTiltAndPosition();
    }

    void Update()
    {
        //the curve moves as the player moves, so we need to get new point positions
        GetBezierCurvePointPositions();
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            zoomInTicks = 10; ;

         if ( zoomInTicks > 0)
        {
            TiltAndMoveAlongCurve(CameraConstants.Instance.INCREMENT_T);
            zoomInTicks--;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 )
            zoomOutTicks = 10;

        if (zoomOutTicks > 0)
        {
            TiltAndMoveAlongCurve(CameraConstants.Instance.DECREMENT_T);
            zoomOutTicks--;
        }
    }

    private void GetBezierCurvePointPositions()
    {
        p1 = route.GetChild(0).position;
        p2 = route.GetChild(1).position;
        p3 = route.GetChild(2).position;
        p4 = route.GetChild(3).position;
    }

    private void InitializeCameraTiltAndPosition()
    {
            while (cam.rotation.eulerAngles.x > CameraConstants.Instance.INITIAL_ZOOM_ANGLE_X_AXIS)
            {
                tPos = AdjustParameter(CameraConstants.Instance.INCREMENT_T, tPos,TILT);
                cam.position = MathUtilBasic.CalcCurrPosAlongTheCurve(tPos, p1, p2, p3, p4);
                tRot = AdjustParameter(CameraConstants.Instance.INCREMENT_T, tRot,TRANSLATION);
                cam.rotation = MathUtilBasic.CalcRotationChangeAlongTheCurve(tRot, rotationRoute.GetChild(0), rotationRoute.GetChild(1));
            }
    }

    private static float AdjustParameter(bool isToBeIncremented,float tParam, string typeOfParameter)
    {
        if (typeOfParameter.Equals(TILT))
        {
            if (isToBeIncremented)
                tParam += Time.deltaTime * CameraConstants.Instance.TILT_SPEED;
            else
                tParam -= Time.deltaTime * CameraConstants.Instance.TILT_SPEED;
            tParam = Mathf.Clamp(tParam, CameraConstants.Instance.TILT_START, CameraConstants.Instance.TILT_END);
        }
        else if (typeOfParameter.Equals(TRANSLATION))
        {
            if (isToBeIncremented)
                tParam += Time.deltaTime * CameraConstants.Instance.CURVE_MOVE_SPEED;
            else
                tParam -= Time.deltaTime * CameraConstants.Instance.CURVE_MOVE_SPEED;
            tParam = Mathf.Clamp(tParam, CameraConstants.Instance.CURVE_START, CameraConstants.Instance.CURVE_END);
        }

        return tParam;
    }
    private void TiltAndMoveAlongCurve(bool isToBeInCremented)
    {
        tPos = AdjustParameter(isToBeInCremented, tPos, TRANSLATION);
        cam.position = MathUtilBasic.CalcCurrPosAlongTheCurve(tPos, p1, p2, p3, p4);
        tRot = AdjustParameter(isToBeInCremented, tRot, TILT);
        cam.rotation = MathUtilBasic.CalcRotationChangeAlongTheCurve(tRot, rotationRoute.GetChild(0), rotationRoute.GetChild(1));

    }
}