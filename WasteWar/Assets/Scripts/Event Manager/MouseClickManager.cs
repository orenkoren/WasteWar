using UnityEngine;

public class MouseClickManager : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;
    [SerializeField]
    private Camera cam;

    private RaycastHit hit;
    private Ray ray;
    private Vector3 terrainSize;
    private int TemplateStructureType { get; set; }

    void Start()
    {
        terrainSize = terrain.terrainData.size;
        GameEvents.TemplateSelectedTypeListeners += SetTemplateStructureType;
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (
           Input.GetMouseButtonDown(0)
           && Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
           && MathUtilBasic.CursorIsWithinBounds(hit.point, terrainSize)
           )
        {
            GameEvents.FireStructurePlaced(this, hit.point,TemplateStructureType);
        }
    }
    void SetTemplateStructureType(object sender, StructureType type)
    {
        TemplateStructureType = (int)type; 
    }
}
