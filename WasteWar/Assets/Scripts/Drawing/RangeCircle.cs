using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeCircle : MonoBehaviour
{
    private const float LINE_ALTITUDE_ADJUSTER=0.1f;

    [Range(0, 100)]
    public int segments = 100;
    [Range(0, 5)]
    public float xradius = 5;
    [Range(0, 5)]
    public float yradius = 5;
    LineRenderer line;
    private float halfYSize; 

    void Start()
    {
        InitializeLineData();
        CreateCircleVerteces();
    }

    void CreateCircleVerteces()
    {
        float x;
        //float y;
        float z;

        float angle = 0f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;
            line.SetPosition(i, new Vector3(x, halfYSize,  z));

            angle += (360f / segments);
        }
    }

    void InitializeLineData()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        line.material.color = Color.green;
        halfYSize = -(gameObject.GetComponent<MeshFilter>().mesh.bounds.size.y / 2 - LINE_ALTITUDE_ADJUSTER);
    }
}