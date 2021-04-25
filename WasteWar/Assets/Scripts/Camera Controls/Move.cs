using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;
    [SerializeField]
    private Transform cam;
  
    private Vector3 terrainSize;

    void Start()
    {
        terrainSize = terrain.terrainData.size;
       // CenterCamera();

        //this probably won't work if you resize the game window at runtime
        ScreenAndMapValues.Initializer(Screen.height - CameraConstants.Instance.SCREEN_BORDER,
                                          CameraConstants.Instance.SCREEN_BORDER,
                                          Screen.width - CameraConstants.Instance.SCREEN_BORDER,
                                          CameraConstants.Instance.SCREEN_BORDER,
                                          terrainSize.z - CameraConstants.Instance.WORLD_BORDER,
                                          CameraConstants.Instance.WORLD_BORDER,
                                          terrainSize.x - CameraConstants.Instance.WORLD_BORDER,
                                          CameraConstants.Instance.WORLD_BORDER);
    }

    void Update()
    {
        TryToMoveForward();
        TryToMoveBack();
        TryToMoveRight();
        TryToMoveLeft();
    }

    private void TryToMoveForward()
    {
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= ScreenAndMapValues.TOP_SCREEN_EDGE)
            cam.position = MoveXZ(cam.position, cam.forward);
    }

    private void TryToMoveBack()
    {
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= ScreenAndMapValues.BOTTOM_SCREEN_EDGE)
            cam.position = MoveXZ(cam.position, -cam.forward);
    }

    private void TryToMoveRight()
    {
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= ScreenAndMapValues.RIGHT_SCREEN_EDGE)
            cam.position = MoveXZ(cam.position, cam.right);
    }

    private void TryToMoveLeft()
    {
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= ScreenAndMapValues.LEFT_SCREEN_EDGE)
            cam.position = MoveXZ(cam.position, -cam.right);
    }

    private void CenterCamera()
    {
        cam.position = new Vector3(terrainSize.x / 2, CameraConstants.Instance.DEFAULT_CAMERA_ALTITUDE, terrainSize.z / 2);
    }
    private Vector3 MoveXZ(Vector3 pos,Vector3 directionVec)
    {
        pos.x += directionVec.x * CameraConstants.Instance.MOVE_SPEED * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, ScreenAndMapValues.BOTTOM_MAP_EDGE, ScreenAndMapValues.TOP_MAP_EDGE);
        pos.y += directionVec.y * CameraConstants.Instance.MOVE_SPEED * Time.deltaTime;
        pos.z += directionVec.z * CameraConstants.Instance.MOVE_SPEED * Time.deltaTime;
        pos.z = Mathf.Clamp(pos.z, ScreenAndMapValues.BOTTOM_MAP_EDGE, ScreenAndMapValues.TOP_MAP_EDGE);

        return new Vector3(pos.x, pos.y, pos.z);
    }
}

public static class ScreenAndMapValues
{
    public static float TOP_SCREEN_EDGE { get; private set; }
    public static float BOTTOM_SCREEN_EDGE { get; private set; }
    public static float RIGHT_SCREEN_EDGE{ get; private set; }
    public static float LEFT_SCREEN_EDGE { get; private set; }

    public static float TOP_MAP_EDGE { get; private set; }
    public static float BOTTOM_MAP_EDGE { get; private set; }
    public static float RIGHT_MAP_EDGE { get; private set; }
    public static float LEFT_MAP_EDGE { get; private set; }

    public static void Initializer(float top_screen_edge, float bottom_screen_edge, float right_screen_edge, float left_screen_edge, float top_map_edge, float bottom_map_edge, float right_map_edge, float left_map_edge)
    {
        TOP_SCREEN_EDGE = top_screen_edge;
        BOTTOM_SCREEN_EDGE = bottom_screen_edge;
        RIGHT_SCREEN_EDGE = right_screen_edge;
        LEFT_SCREEN_EDGE = left_screen_edge;
        TOP_MAP_EDGE = top_map_edge;
        BOTTOM_MAP_EDGE = bottom_map_edge;
        RIGHT_MAP_EDGE = right_map_edge;
        LEFT_MAP_EDGE = left_map_edge;
    }
}