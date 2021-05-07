using UnityEngine;

public class MouseOverManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    RuntimeGameObjRefs runtimeGameObjRefs;


    private Terrain terrain;
    private RaycastHit hit;
    private Ray ray;
    private Vector3 terrainSize;

    private void Start()
    {
        terrain = runtimeGameObjRefs.terrain;
        terrainSize = terrain.terrainData.size;
    }

    private void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
           && MathUtils.CursorIsWithinBounds(hit.point, terrainSize))
            GameEvents.FireMouseOver(this, hit.point);
    }
}
