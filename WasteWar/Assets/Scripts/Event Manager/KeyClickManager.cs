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
    private GameObject prefabPipe;
    [SerializeField]
    private GameObject prefabPowerPole;
    [SerializeField]
    private Terrain terrain;
    [SerializeField]
    private Camera cam;

    private RaycastHit hit;
    private Ray ray;

    private void Start()
    {
    }

    void Update()
    {
        foreach (var key in GameKeys.Instance.StructureKeybinds)
        {
            if (Input.GetKeyDown(key))
            {
                ray = cam.ScreenPointToRay(Input.mousePosition);

                // if mouse on the ground/ and cursor within bounds
                if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND) 
                    && MathUtils.CursorIsWithinBounds(hit.point, terrain.terrainData.size))
                {
                    switch (key)
                    {
                        case KeyCode.B:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabBuilding, mousePos = hit.point});
                            break;
                        //TODO turret placement doesn't work (other placements do) (because of Turret components or something)... fix.
                        case KeyCode.V:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabTurret, mousePos = hit.point});
                            break;
                        case KeyCode.C:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabWall, mousePos = hit.point});
                            break;
                        case KeyCode.X:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabPipe, mousePos = hit.point});
                            break;
                        case KeyCode.Z:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabPowerPole, mousePos = hit.point});
                            break;
                        case KeyCode.Escape:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = null, mousePos = new Vector3( 0, 0, 0 )});
                            break;
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
            GameEvents.FireBuildingRotation(this,5);
    }
}
