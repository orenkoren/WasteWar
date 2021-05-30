using UnityEngine;
public class Zoom : MonoBehaviour
{
    private const int ZOOM_TICKS_PER_FRAME = 10;

    [SerializeField]
    private float FovZoomSpeed;
    [SerializeField]
    private Transform route;
    [SerializeField]
    private Camera cam1;
    [SerializeField]
    private Camera cam2;
    [SerializeField]
    private Transform rotationRoute;

    public float tRot = 0;
    public float tPos = 0;

    public Camera Cam { get; set; }
    private int zoomInTicks = ZOOM_TICKS_PER_FRAME;
    private int zoomOutTicks = ZOOM_TICKS_PER_FRAME;
    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;
    private Vector3 p4;

    private void Awake()
    {
        Cam = cam1;
        Cam.GetComponent<AudioListener>().enabled = true;
        cam1.enabled = true;
        cam2.enabled = false;
    }

    private void Start()
    {
        GameEvents.MiddleMouseClickPressedListeners += SetCamera;
        GetBezierCurvePointPositions();
        InitializeCameraTiltAndPosition();
    }

    private void Update()
    {
        //the curve moves as the player moves, so we need to get new point positions
        if (Cam == cam1)
        {
            GetBezierCurvePointPositions();
            TiltAndMoveAlongCurvePerTick();
        }
        else if (Cam == cam2)
        {
            FoVZoom();
        }
    }

    public void SetCamera(object sender, int data)
    {
        if (Cam == cam1)
        {
            Cam.GetComponent<AudioListener>().enabled = false;
            cam1.enabled = false;
            cam2.enabled = true;
            Cam = cam2;
            Cam.GetComponent<AudioListener>().enabled = true;
        }
        else
        {
            Cam.GetComponent<AudioListener>().enabled = false;
            cam1.enabled = true;
            cam2.enabled = false;
            Cam = cam1;
            Cam.GetComponent<AudioListener>().enabled = true;
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
        while (Cam.transform.rotation.eulerAngles.x > CameraConstants.Instance.INITIAL_ZOOM_ANGLE_X_AXIS)
        {
            tPos = AdjustParameter(CameraConstants.Instance.INCREMENT_T, tPos, CameraConstants.TRANSLATION);
            Cam.transform.position = MathUtils.CalcCurrPosAlongTheCurve(tPos, p1, p2, p3, p4);
            tRot = AdjustParameter(CameraConstants.Instance.INCREMENT_T, tRot, CameraConstants.TILT);
            Cam.transform.rotation = MathUtils.CalcRotationChangeAlongTheCurve(tRot, rotationRoute.GetChild(0), rotationRoute.GetChild(1));
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
        Cam.transform.position = MathUtils.CalcCurrPosAlongTheCurve(tPos, p1, p2, p3, p4);
        tRot = AdjustParameter(isToBeInCremented, tRot, CameraConstants.TILT);
        Cam.transform.rotation = MathUtils.CalcRotationChangeAlongTheCurve(tRot, rotationRoute.GetChild(0), rotationRoute.GetChild(1));
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
    private void FoVZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Cam.fieldOfView < 140f)
            Cam.fieldOfView += FovZoomSpeed;
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && Cam.fieldOfView > 30f)
            Cam.fieldOfView -= FovZoomSpeed;
    }

    private void OnDestroy()
    {
        GameEvents.MiddleMouseClickPressedListeners -= SetCamera;
    }
}
