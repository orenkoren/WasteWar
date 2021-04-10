using UnityEngine;

public class MouseOverManager : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;
    [SerializeField]
    private Camera cam;

    private RaycastHit hit;
    private Ray ray;
    private Vector3 terrainSize;

    // Start is called before the first frame update
    void Start()
    {
        terrainSize = terrain.terrainData.size;
    }

    // Update is called once per frame
    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
           && MathUtils.CursorIsWithinBounds(hit.point, terrainSize))
            GameEvents.FireMouseOver(this, hit.point);
    }
}
