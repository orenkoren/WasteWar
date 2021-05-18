using UnityEngine;

public class MouseClickManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    RuntimeGameObjRefs runtimeGameObjRefs;


    private bool isCursorLocked = false;
    private Terrain terrain;
    private RaycastHit hit;
    private Ray ray;
    private Vector3 terrainSize;
    private TemplateData templateData;

    private void Start()
    {
        terrain = runtimeGameObjRefs.terrain;
        templateData = new TemplateData();
        terrainSize = terrain.terrainData.size;
        GameEvents.TemplateSelectedListeners += SetTemplateData;
    }

    private void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (
           Input.GetMouseButtonDown(0)
           && templateData.TemplateStructure != null
           && !templateData.TemplateStructure.tag.Contains("Pipe")
           && Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
           && MathUtils.CursorIsWithinBounds(hit.point, terrainSize)
           )
        {
            templateData.MousePos = hit.point;
            GameEvents.FireLeftClickPressed(this, templateData);
        }
        else if (
          Input.GetMouseButtonDown(1)
          && templateData.TemplateStructure == null
          && Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.ATTACKABLE)
          && MathUtils.CursorIsWithinBounds(hit.point, terrainSize)
          )
        {
            GameEvents.FireRightClickPressed(this, hit);
        }
        else if (
            Input.GetMouseButton(0)
            && templateData.TemplateStructure != null
            && templateData.TemplateStructure.tag.Contains("Pipe")
            && Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
            && MathUtils.CursorIsWithinBounds(hit.point, terrainSize)
           )
        {
            if (!isCursorLocked)
            {
                isCursorLocked = true;
                templateData.MousePos = hit.point;
            }
            else
            {
                if (Mathf.Abs(hit.point.x - templateData.MousePos.x) < 2 && templateData.TemplateStructure.CompareTag("PipeTBTemplate"))
                {
                    templateData.MousePos = new Vector3(templateData.MousePos.x, hit.point.y, hit.point.z);
                }
                else if (Mathf.Abs(hit.point.z - templateData.MousePos.z) < 2 && templateData.TemplateStructure.CompareTag("PipeLRTemplate"))
                {
                    templateData.MousePos = new Vector3(hit.point.x, hit.point.y, templateData.MousePos.z);
                }
                else
                    templateData.MousePos = hit.point;
            }

            GameEvents.FireContinuousLeftClickPress(this, templateData);
        }

        if (Input.GetMouseButtonUp(0)
            && templateData.TemplateStructure != null
            && templateData.TemplateStructure.tag.Contains("Pipe"))
            isCursorLocked = false;
    }

    private void SetTemplateData(object sender, TemplateData data)
    {
        templateData = data;
    }

    private void OnDestroy()
    {
        GameEvents.TemplateSelectedListeners -= SetTemplateData;
    }
}
