using UnityEngine;
public class Zoom : MonoBehaviour
{

    private const int ZOOM_TICKS_PER_FRAME = 10;

    [SerializeField]
    private Transform route;

    [SerializeField]
    private Transform cam;

    [SerializeField]
    private Transform rotationRoute;


    public float tRot = 0;
    public float tPos = 0;

    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;
    private Vector3 p4;

    private int zoomInTicks = ZOOM_TICKS_PER_FRAME;
    private int zoomOutTicks = ZOOM_TICKS_PER_FRAME;

    void Start()
    {
        GetBezierCurvePointPositions();
        InitializeCameraTiltAndPosition();
    }

    void Update()
    {
        //the curve moves as the player moves, so we need to get new point positions
        GetBezierCurvePointPositions();
        TiltAndMoveAlongCurvePerTick();
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
            tPos = AdjustParameter(CameraConstants.Instance.INCREMENT_T, tPos, CameraConstants.TRANSLATION);
            cam.position = MathUtilBasic.CalcCurrPosAlongTheCurve(tPos, p1, p2, p3, p4);
            tRot = AdjustParameter(CameraConstants.Instance.INCREMENT_T, tRot, CameraConstants.TILT);
            cam.rotation = MathUtilBasic.CalcRotationChangeAlongTheCurve(tRot, rotationRoute.GetChild(0), rotationRoute.GetChild(1));
        }
    }

    private float AdjustParameter(bool isToBeIncremented,float inputParameter, string paramType)
    {
        float tParam = inputParameter;

        CalcParameter(paramType);

        return tParam;

        float CalcParameter(string parType)
        {
            if (isToBeIncremented)
                tParam += Time.deltaTime * CameraConstants.Instance.GetSpeedOfCamTransform(parType);
            else
                tParam -= Time.deltaTime * CameraConstants.Instance.GetSpeedOfCamTransform(parType);
            tParam = Mathf.Clamp(tParam, CameraConstants.Instance.GetStartPosOfCamTransform(parType), CameraConstants.Instance.GetEndPosOfCamTransform(parType));
            return tParam;
        }
    }
    
    private void TiltAndMoveAlongCurve(bool isToBeInCremented)
    {
        tPos = AdjustParameter(isToBeInCremented, tPos, CameraConstants.TRANSLATION);
        cam.position = MathUtilBasic.CalcCurrPosAlongTheCurve(tPos, p1, p2, p3, p4);
        tRot = AdjustParameter(isToBeInCremented, tRot, CameraConstants.TILT);
        cam.rotation = MathUtilBasic.CalcRotationChangeAlongTheCurve(tRot, rotationRoute.GetChild(0), rotationRoute.GetChild(1));
    }

    private void TiltAndMoveAlongCurvePerTick()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            zoomInTicks = ZOOM_TICKS_PER_FRAME;

        if (zoomInTicks > 0)
        {
            TiltAndMoveAlongCurve(CameraConstants.Instance.INCREMENT_T);
            zoomInTicks--;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            zoomOutTicks = ZOOM_TICKS_PER_FRAME;

        if (zoomOutTicks > 0)
        {
            TiltAndMoveAlongCurve(CameraConstants.Instance.DECREMENT_T);
            zoomOutTicks--;
        }
    }
}
