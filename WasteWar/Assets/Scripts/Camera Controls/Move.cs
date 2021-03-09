using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    [SerializeField]
    private Transform terrain;
    [SerializeField]
    private Transform playerCamera;
    private Vector3 terrainSize;


    // Start is called before the first frame update
    void Start()
    {
        terrainSize = terrain.GetComponent<Terrain>().terrainData.size;

        //setting camera to the center of terrain
        var cameraStartingPos = new Vector3(terrainSize.x / 2, CameraConstants.DEFAULT_HEIGHT, terrainSize.z / 2);

        transform.position = cameraStartingPos;

    }

    void Update()
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
