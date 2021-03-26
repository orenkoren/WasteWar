using Constants;
using UnityEngine;

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
    private DrawOnTerrain terrainCanvas;

    private bool isAStructureSelected;
    bool isSpaceOccupied = false;
    bool hasFired = false;

    void Start()
    {
        terrainCanvas = drawOnTerrain;
        terrainSize = terrain.GetComponent<Terrain>().terrainData.size;
        structureGrid = new StructureGrid(terrainSize.x, terrainSize.z, GridConstants.Instance.FloatCellSize());
    }

    private void ApplyStructureGrid(object sender, bool isDrawing)
    {
        if (!isDrawing)
            return;


    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
            && CursorIsWithinBounds(hit.point))
        {
            PlaceStructureOnGridOnClick();
            hasFired = true;
        }
        else
            hasFired = false;
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
        isAStructureSelected = terrainCanvas.DrawTemplateStructure(hit.point); // make a DrawStateManager class/script for this part

        if (isAStructureSelected == true)
        {
            isSpaceOccupied = structureGrid.IsGridCellFilled(hit.point, terrainCanvas.TemplateStructureSize); // make a CellAvailability

            if (!isSpaceOccupied)
                terrainCanvas.SetTemplateStructureColor(Color.green);
            else
                terrainCanvas.SetTemplateStructureColor(Color.red);
            terrainCanvas.SetTemplateStructurePos(hit.point);
        }

        if (Input.GetMouseButtonDown(0) && isAStructureSelected == true && !isSpaceOccupied) // make a FinalBuildingManager 
        {
            terrainCanvas.DrawStructure(hit.point);
            structureGrid.AddStructure(hit.point, terrainCanvas.TemplateStructureSize);
            terrainCanvas.DestroyTemplateStructure();
        }
    }

}
