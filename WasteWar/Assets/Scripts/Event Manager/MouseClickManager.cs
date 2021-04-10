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
    private TemplateData templateData;

    void Start()
    {
        templateData = new TemplateData();
        terrainSize = terrain.terrainData.size;
        GameEvents.TemplateSelectedListeners += SetTemplateData;
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (
           Input.GetMouseButtonDown(0)
           && templateData.StructureType != StructureType.NONE
           && Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
           && MathUtils.CursorIsWithinBounds(hit.point, terrainSize)
           )
        {
            templateData.mousePos = hit.point;
            GameEvents.FireLeftClickPressed(this, templateData);
        }
        else if  (
          Input.GetMouseButtonDown(1)
          && templateData.StructureType == StructureType.NONE
          && Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.ATTACKABLE)
          && MathUtils.CursorIsWithinBounds(hit.point, terrainSize)
          )
        {
            GameEvents.FireRightClickPressed(this, hit);
        }
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
