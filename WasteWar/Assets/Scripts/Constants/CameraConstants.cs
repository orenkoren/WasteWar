using UnityEngine;


//need an explanation as to how this works
public class CameraConstants : MonoBehaviour
{
    public float MOVE_SPEED = 20f;
    public float ROTATION_SPEED = 60f;
    public float SCREEN_BORDER = 20f;
    public float DEFAULT_CAMERA_ALTITUDE = 20f;
    public float WORLD_BORDER = 10f;
    //method that relies on this variable fires on scrolling
    public float INITIAL_ZOOM_ANGLE_X_AXIS = 60f;

    public float TILT_SPEED = 1.25f;
    public float TILT_END = 1f;
    public float TILT_START = 0;

    public float CURVE_MOVE_SPEED = 1.25f;
    public float CURVE_END = 0.95f;
    public float CURVE_START = 0f;

    public float RAYCAST_DISTANCE = 100f;

    public bool INCREMENT_T { get; } = true;
    public bool DECREMENT_T { get; } = false;

    public const string TILT = "tilt";
    public const string TRANSLATION = "translation";


    public float GetSpeedOfCamTransform(string transformType)
    {
        if (TILT.Equals(transformType))
            return TILT_SPEED;
        else
            return CURVE_MOVE_SPEED;
    }
    public float GetEndPosOfCamTransform(string transformType)
    {
        if (TILT.Equals(transformType))
            return TILT_END;
        else
            return CURVE_END;
    }
    public float GetStartPosOfCamTransform(string transformType)
    {
        if (TILT.Equals(transformType))
            return TILT_START;
        else
            return CURVE_START;
    }

    public static CameraConstants Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //what's this here?
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
