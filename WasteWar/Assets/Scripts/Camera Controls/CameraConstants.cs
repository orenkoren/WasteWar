using UnityEngine;


//need an explanation as to how this works
public class CameraConstants : MonoBehaviour
{
    public  float MOVE_SPEED = 20f;
    public  float ROTATION_SPEED = 60f;
    public  float SCREEN_BORDER = 20f;
    public  float DEFAULT_HEIGHT = 5f;
    public  float WORLD_BORDER = 10f;
    //method that relies on this variable fires on scrolling
    public float INITIAL_ZOOM_ANGLE_X_AXIS = 60f;

    public float TILT_SPEED = 1.25f;
    public float TILT_END = 1f;
    public float TILT_START = 0;

    public float CURVE_MOVE_SPEED = 1.25f;
    public  float CURVE_END = 0.95f;
    public  float CURVE_START = 0f;

    public bool INCREMENT_T { get; } = true;
    public bool DECREMENT_T { get; } = false;

    public static CameraConstants Instance { get; private set; }
    
    //????????
    private void Awake()
    {
        if (Instance == null)
        {
            //what's this here?
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
       
        else
        {
            Destroy(gameObject);
        }
    }
}

