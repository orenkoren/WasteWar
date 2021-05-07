using Constants;
using UnityEngine;

public class KeyClickManager : MonoBehaviour
{
    [SerializeField]
    private PrefabTemplates templates;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    RuntimeGameObjRefs runtimeGameObjRefs;

    private Terrain terrain;
    private GameObject prefabPipe;
    private RaycastHit hit;
    private Ray ray;

    private bool isCurvedRotationModeOn = false;

    private void Start()
    {
        terrain = runtimeGameObjRefs.terrain;
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
            GameEvents.FireBuildingRotation(this, isCurvedRotationModeOn);
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isCurvedRotationModeOn)
            {
                prefabPipe = templates.PipeTopRight;
                GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabPipe, MousePos = hit.point });
                isCurvedRotationModeOn = true;
            }
            //something doesn't work well, because of the scuffed curved pipe model, also implement rotation resetter
            else
            {
                prefabPipe = templates.PipeLeftRight;
                GameEvents.FireTemplateSelected(this, new TemplateData { TemplateStructure = prefabPipe, MousePos = hit.point });
                isCurvedRotationModeOn = false;
            }
        }
    }

    private void SetPipePrefab(object sender, string prefabName)
    {
        if (prefabName.Equals("PipeTBTemplate"))
            prefabPipe = templates.PipeTopBottom;
        else if (prefabName.Equals("PipeLRTemplate"))
            prefabPipe = templates.PipeLeftRight;
        else if (prefabName.Equals("PipeTRTemplate"))
            prefabPipe = templates.PipeTopRight;
        else if (prefabName.Equals("PipeBRTemplate"))
            prefabPipe = templates.PipeBottomRight;
        else if (prefabName.Equals("PipeBLTemplate"))
            prefabPipe = templates.PipeBottomLeft;
        else if (prefabName.Equals("PipeTLTemplate"))
            prefabPipe = templates.PipeTopLeft;
    }
}
