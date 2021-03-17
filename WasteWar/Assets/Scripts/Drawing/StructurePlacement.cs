using UnityEngine;
using Constants;

public class StructurePlacement : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private DrawOnTerrain drawOnTerrain;
    
    private RaycastHit hit;
    private Ray ray;

    private StructureGrid structureGrid;
    private Vector3 terrainSize;
    DrawOnTerrain terrainCanvas;

    private bool isAStructureSelected;
    bool isSpaceOccupied = false;

    void Start()
    {
        terrainCanvas = drawOnTerrain;
        terrainSize = terrain.GetComponent<Terrain>().terrainData.size;
        structureGrid = new StructureGrid(terrainSize.x,terrainSize.z, GridConstants.Instance.FloatCellSize());
    }

    void Update() {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND) && CursorIsWithinBounds(hit.point))
            PlaceStructureOnGridOnClick();
    }

    private bool CursorIsWithinBounds(Vector3 hitLocation)
    {
        int failedConditions = 0;
        //TODO unnecessary to go through all 1, should short circuit if any condition is true
        failedConditions = hitLocation.x > terrainSize.z ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.x < 0 ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.z > terrainSize.z ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.z < 0 ? (failedConditions + 1) : failedConditions;

        return failedConditions == 0;
    }

    private void PlaceStructureOnGridOnClick()
    {
        isAStructureSelected=terrainCanvas.DrawTemplateStructure(hit.point);

        if (isAStructureSelected == true)
        {
            isSpaceOccupied = structureGrid.IsGridCellFilled(hit.point, terrainCanvas.TemplateStructureSize);
           
            if (!isSpaceOccupied)
                terrainCanvas.SetTemplateStructureColor(Color.green);
            else
                terrainCanvas.SetTemplateStructureColor(Color.red);
            terrainCanvas.SetTemplateStructurePos(hit.point);
        }

        if (Input.GetMouseButtonDown(0) && isAStructureSelected == true && !isSpaceOccupied)
        {
            terrainCanvas.DrawStructure(hit.point);
            structureGrid.AddStructure(hit.point, terrainCanvas.TemplateStructureSize);
            terrainCanvas.DestroyTemplateStructure(); 
        }
    }

}
