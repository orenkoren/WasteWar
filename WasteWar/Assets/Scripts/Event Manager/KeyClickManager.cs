using UnityEngine;
using Constants;

public class KeyClickManager : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabBuilding;
    [SerializeField]
    private GameObject prefabTurret;
    [SerializeField]
    private GameObject prefabWall;
    [SerializeField]
    private Terrain terrain;
    [SerializeField]
    private Camera cam;

    private RaycastHit hit;
    private Ray ray;
    private Vector3 terrainSize;

    private void Start()
    {
        terrainSize = terrain.terrainData.size;
    }

    void Update()
    {
        foreach (var key in GameKeys.Instance.StructureKeybinds)
        {
            if (Input.GetKeyDown(key))
            {
                ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND) && MathUtilBasic.CursorIsWithinBounds(hit.point, terrainSize))
                {
                    switch (key)
                    {
                        case KeyCode.B:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabBuilding, mousePos = hit.point, StructureType=StructureType.BUILDING});
                            break;
                        case KeyCode.V:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabTurret, mousePos = hit.point, StructureType = StructureType.TURRET });
                            break;
                        case KeyCode.C:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabWall, mousePos = hit.point, StructureType = StructureType.WALL });
                            break;
                        case KeyCode.Escape:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = null, mousePos = new Vector3( 0, 0, 0 ), StructureType = StructureType.NONE });
                            break;
                    }
                }
            }
        }
    }
}
