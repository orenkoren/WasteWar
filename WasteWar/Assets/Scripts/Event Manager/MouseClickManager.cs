using UnityEngine;

public class MouseClickManager : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Terrain terrain;
    private RaycastHit hit;
    private Ray ray;
    private Vector3 terrainSize;
    private TemplateData templateData;

    private void Start()
    {
        terrain = RuntimeGameObjRefs.Instance.TERRAIN;
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
            templateData.MousePos = hit.point;
            GameEvents.FireContinuousLeftClickPress(this, templateData);
        }

        if (Input.GetMouseButtonUp(0)
            && templateData.TemplateStructure != null
            && templateData.TemplateStructure.tag.Contains("Pipe"))
            GameEvents.FireLeftClickUp(this, 5);
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
