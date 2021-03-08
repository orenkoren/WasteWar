using UnityEngine;

static class CameraConstants
{
    public const float MOVE_SPEED = 20f;
    public const float ROTATION_SPEED = 60f;
    public const float ZOOM_SPEED = 5f;
    public const float SCREEN_EDGE = 20f;
    public const float DEFAULT_HEIGHT = 5f;
    public const float WORLD_EDGE = 10f;
    public const float X_ANGLE = 60f;
    public const int MIN_PARAMETER = 0;
    public const int MAX_PARAMETER = 140;
}

public class CameraController : MonoBehaviour
{ private Vector3 terrainSize;
  //have to make parameter int because of floating point errors
  private int t = 0;
   

    // Start is called before the first frame update
    void Start()
    {
        //setting default rotation of the player camera
        GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().Rotate(CameraConstants.X_ANGLE, 0, 0);


        var terrain = GameObject.FindGameObjectWithTag("Terrain");
        terrainSize = terrain.GetComponent<Terrain>().terrainData.size;

        //setting camera to the center of terrain
        var cameraStartingPos = new Vector3(terrainSize.x / 2, CameraConstants.DEFAULT_HEIGHT, terrainSize.z / 2);

        transform.position = cameraStartingPos;
        
    }

    // Update is called once per frame
    void Update()
    {
        //access modifiers don't work on variables declared here?
        MoveCamera();
        RotateCamera();
        ZoomCamera();
    }

    void ZoomCamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && t > CameraConstants.MIN_PARAMETER)
        {
                t -= 1;
            transform.Translate(0,1,0,Space.Self);
                transform.position = new Vector3(
                    transform.position.x,
                    5 * Mathf.Cos(t/100f),
                    transform.position.z
                    );
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && t < CameraConstants.MAX_PARAMETER)
        {
            t += 1;
            transform.position = new Vector3(
                transform.position.x,
                5 * Mathf.Cos(t/100f),
                transform.position.z
                );
        }
        Debug.Log(Mathf.Cos(t/100f));
        Debug.Log(t);
        Debug.Log(transform.forward);
    }

    void RotateCamera()
    {
        if (Input.GetKey("q"))
            transform.Rotate(Vector3.up, -CameraConstants.ROTATION_SPEED * Time.deltaTime);
        else if (Input.GetKey("e"))
            transform.Rotate(Vector3.up, CameraConstants.ROTATION_SPEED * Time.deltaTime);
    }

    void MoveCamera()
    {

        //touching scale slider fucks this up...why??
        Vector3 pos = transform.position;
        if (pos.z < (terrainSize.z - CameraConstants.WORLD_EDGE) && (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - CameraConstants.SCREEN_EDGE))
        {
            pos.z += CameraConstants.MOVE_SPEED * Time.deltaTime;
        }
        if (pos.z > CameraConstants.WORLD_EDGE && (Input.GetKey("s") || Input.mousePosition.y <= CameraConstants.SCREEN_EDGE))
            pos.z -= CameraConstants.MOVE_SPEED * Time.deltaTime;
        if (pos.x < (terrainSize.x - CameraConstants.WORLD_EDGE) && (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - CameraConstants.SCREEN_EDGE))
            pos.x += CameraConstants.MOVE_SPEED * Time.deltaTime;
        if (pos.x > CameraConstants.WORLD_EDGE && (Input.GetKey("a") || Input.mousePosition.x <= CameraConstants.SCREEN_EDGE))
            pos.x -= CameraConstants.MOVE_SPEED * Time.deltaTime;

        transform.position = pos;
    }
}
