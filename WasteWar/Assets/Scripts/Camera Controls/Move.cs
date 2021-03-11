using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;
    private Vector3 terrainSize;
    private Vector3 camPosition;
    private Transform cam;


    // Start is called before the first frame update
    void Start()
    {
        terrainSize = terrain.terrainData.size;

        //setting cam to the center of terrain
        transform.position = new Vector3(terrainSize.x / 2, CameraConstants.DEFAULT_HEIGHT, terrainSize.z / 2);
        
        //variables for readability
        cam = transform;
        camPosition = transform.position;
    }

    void Update()
    {
        if (camPosition.z < (terrainSize.z - CameraConstants.WORLD_BORDER)
            && (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - CameraConstants.SCREEN_BORDER)) 
            cam.Translate(cam.forward* CameraConstants.MOVE_SPEED * Time.deltaTime,Space.World);
        if (camPosition.z > CameraConstants.WORLD_BORDER &&
            (Input.GetKey("s") || Input.mousePosition.y <= CameraConstants.SCREEN_BORDER))
            cam.Translate(-cam.forward * CameraConstants.MOVE_SPEED * Time.deltaTime, Space.World);
        if (camPosition.x < (terrainSize.x - CameraConstants.WORLD_BORDER) &&
            (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - CameraConstants.SCREEN_BORDER))
            cam.Translate(cam.right * CameraConstants.MOVE_SPEED * Time.deltaTime,Space.World);
        if (camPosition.x > CameraConstants.WORLD_BORDER && 
            (Input.GetKey("a") || Input.mousePosition.x <= CameraConstants.SCREEN_BORDER))
            cam.Translate(-cam.right * CameraConstants.MOVE_SPEED * Time.deltaTime, Space.World);
    }
}
