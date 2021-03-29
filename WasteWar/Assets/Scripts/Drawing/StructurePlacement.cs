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

    private GameGrid gameGrid;
    private Vector3 terrainSize;
    private DrawOnTerrain terrainCanvas;

    void Start()
    {
        terrainCanvas = drawOnTerrain;
        terrainSize = terrain.GetComponent<Terrain>().terrainData.size;
        gameGrid = new GameGrid(terrainSize.x, terrainSize.z, GridConstants.Instance.FloatCellSize());
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
            && MathUtilBasic.CursorIsWithinBounds(hit.point, terrainSize))
            PlaceStructureOnGridOnClick();
    }

    private void PlaceStructureOnGridOnClick()
    {
        terrainCanvas.IsCellOccupied = gameGrid.IsGridCellFilled(hit.point, terrainCanvas.TemplateStructureSize);

        if (terrainCanvas.IsAStructureToBuildSelected)
            terrainCanvas.DrawTemplateStructure(hit.point);
  
        if (Input.GetMouseButtonDown(0) && terrainCanvas.IsAStructureToBuildSelected == true && !terrainCanvas.IsCellOccupied) // make a FinalBuildingManager 
        {
            gameGrid.AddStructure(hit.point, terrainCanvas.TemplateStructureSize);
        }
    }
}
