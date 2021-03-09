using UnityEngine;

public class Rotate : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKey("q"))
            transform.Rotate(Vector3.up, -CameraConstants.ROTATION_SPEED * Time.deltaTime);
        else if (Input.GetKey("e"))
            transform.Rotate(Vector3.up, CameraConstants.ROTATION_SPEED * Time.deltaTime);
    }
}
