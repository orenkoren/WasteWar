using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;
    [SerializeField]
    private Transform cam;

    private Vector3 terrainSize;

    void TryToMoveForward()
    {
        if (cam.position.z < (terrainSize.z - CameraConstants.WORLD_BORDER)
           && (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - CameraConstants.SCREEN_BORDER))
            cam.Translate(cam.forward * CameraConstants.MOVE_SPEED * Time.deltaTime, Space.World);
    }

    void TryToMoveBack()
    {
        if (cam.position.z > CameraConstants.WORLD_BORDER &&
              (Input.GetKey("s") || Input.mousePosition.y <= CameraConstants.SCREEN_BORDER))
            cam.Translate(-cam.forward * CameraConstants.MOVE_SPEED * Time.deltaTime, Space.World);
    }

    void TryToMoveRight()
    {
        if (cam.position.x < (terrainSize.x - CameraConstants.WORLD_BORDER) &&
            (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - CameraConstants.SCREEN_BORDER))
            cam.Translate(cam.right * CameraConstants.MOVE_SPEED * Time.deltaTime, Space.World);
    }

    void TryToMoveLeft()
    {
        if (cam.position.x > CameraConstants.WORLD_BORDER &&
            (Input.GetKey("a") || Input.mousePosition.x <= CameraConstants.SCREEN_BORDER))
            cam.Translate(-cam.right * CameraConstants.MOVE_SPEED * Time.deltaTime, Space.World);
    }


    void CenterCamera()
    {
        terrainSize = terrain.terrainData.size;
        cam.position = new Vector3(terrainSize.x / 2, CameraConstants.DEFAULT_HEIGHT, terrainSize.z / 2);
    }

    // Start is called before the first frame update
    void Start()
    {
        CenterCamera();
    }

    void Update()
    {
        TryToMoveForward();
        TryToMoveBack();
        TryToMoveRight();
        TryToMoveLeft();

    }
}
