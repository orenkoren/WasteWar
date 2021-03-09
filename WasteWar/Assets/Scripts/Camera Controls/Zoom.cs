using UnityEngine;

public class Zoom : MonoBehaviour
{
    [SerializeField]
    private Transform route;

    [SerializeField]
    private Transform rotationRoute;

    public float t = 0;

    private Vector3 p1;

    private Vector3 p2;

    private Vector3 p3;

    private Vector3 p4;

    void Start()
    {
        //refactor
        while (transform.rotation.eulerAngles.x > 60)
        {
            t += Time.deltaTime * CameraConstants.ZOOM_SPEED; ;
            transform.rotation = Quaternion.Lerp(
                    rotationRoute.GetChild(0).rotation,
                    rotationRoute.GetChild(1).rotation,
                    t);
        }

        p1 = route.GetChild(0).position;
        p2 = route.GetChild(1).position;
        p3 = route.GetChild(2).position;
        p4 = route.GetChild(3).position;
    }

    void Update()
    {
        p1 = route.GetChild(0).position;
        p2 = route.GetChild(1).position;
        p3 = route.GetChild(2).position;
        p4 = route.GetChild(3).position;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (t < 0.95f)
            {
                t += Time.deltaTime * CameraConstants.ZOOM_SPEED;


                //90deg,0,0 to 0,0,0
                transform.rotation = Quaternion.Lerp(
                    rotationRoute.GetChild(0).rotation,
                    rotationRoute.GetChild(1).rotation,
                    t);

                //moving the camera along a curve
                transform.position = Mathf.Pow(1 - t, 3) * p1 +
                    3 * Mathf.Pow(1 - t, 2) * t * p2 +
                    3 * (1 - t) * Mathf.Pow(t, 2) * p3 +
                    Mathf.Pow(t, 3) * p4;
            }
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (t > 0)
            {
                t -= Time.deltaTime * CameraConstants.ZOOM_SPEED;

                transform.rotation = Quaternion.Lerp(
                    rotationRoute.GetChild(0).rotation,
                    rotationRoute.GetChild(1).rotation,
                    t);

                //moving the camera along a curve
                transform.position = Mathf.Pow(1 - t, 3) * p1 +
                    3 * Mathf.Pow(1 - t, 2) * t * p2 +
                    3 * (1 - t) * Mathf.Pow(t, 2) * p3 +
                    Mathf.Pow(t, 3) * p4;
            }
        }
    }
}
