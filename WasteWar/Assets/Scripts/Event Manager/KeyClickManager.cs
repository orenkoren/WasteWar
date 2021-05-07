using Constants;
using UnityEngine;

public class KeyClickManager : MonoBehaviour
{
    [SerializeField]
    private PrefabTemplates templates;
    [SerializeField]
    private Terrain terrain;
    [SerializeField]
    private Camera cam;

    private GameObject prefabPipe;
    private RaycastHit hit;
    private Ray ray;

    private void Start()
    {
        GameEvents.PipePlaced2Listeners += SetPipePrefab;
        prefabPipe = templates.PipeTopBottom;
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
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = templates.Building, MousePos = hit.point });
                            break;
                        //TODO turret placement doesn't work (other placements do) (because of Turret components or something)... fix.
                        case KeyCode.V:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = templates.Turret, MousePos = hit.point });
                            break;
                        case KeyCode.C:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = templates.Wall, MousePos = hit.point });
                            break;
                        case KeyCode.X:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabPipe, MousePos = hit.point });
                            break;
                        case KeyCode.Z:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = templates.PowerPole, MousePos = hit.point });
                            break;
                        case KeyCode.Escape:
                            GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = null, MousePos = new Vector3(0, 0, 0) });
                            break;
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
            GameEvents.FireBuildingRotation(this, 5);
    }

    private void SetPipePrefab(object sender, string prefabName)
    {
        if (prefabName.Equals("PipeTBTemplate"))
            prefabPipe = templates.PipeTopBottom;
        else if (prefabName.Equals("PipeLRTemplate"))
            prefabPipe = templates.PipeLeftRight;
    }

    private void OnDestroy()
    {
        GameEvents.PipePlaced2Listeners -= SetPipePrefab;
    }
}
