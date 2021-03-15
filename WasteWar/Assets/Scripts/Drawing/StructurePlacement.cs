using UnityEngine;
using Constants;

public class StructurePlacement : MonoBehaviour
{
    [SerializeField]
    private GameObject buildingPrefab1;
    [SerializeField]
    private GameObject buildingPrefab2;
    [SerializeField]
    private Terrain terrain;
    
    private RaycastHit hit;
    private Ray ray;

    private Vector3 terrainSize;
    private StructureGrid structureGrid;
    private GameObject buildingTemplate;
    private bool isAStructureSelected = false;

    void Start()
    {
        terrainSize = terrain.GetComponent<Terrain>().terrainData.size;
        structureGrid = new StructureGrid(terrainSize.x,terrainSize.z, GridConstants.Instance.FloatCellSize());
    }

    void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.GROUND) && CursorIsWithinBounds(hit.point))
        {
            PlaceStructureOnLogicGridOnClick();
            DrawStructureOnGridOnClick();
        }
    }
    private bool CursorIsWithinBounds(Vector3 hitLocation)
    {
        int failedConditions = 0;
        //unnecessary to go through all 1, should short circuit if any condition is true FIX LATER
        failedConditions = hitLocation.x > terrainSize.z ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.x < 0 ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.z > terrainSize.z ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.z < 0 ? (failedConditions + 1) : failedConditions;

        return failedConditions == 0;
    }
    private void PlaceStructureOnLogicGridOnClick()
    {
        bool isSpaceOccupied=false;

    //will create some kind of switch case for this later on
        if (Input.GetKey(KeyCode.B))
        {
            Destroy(buildingTemplate);
            buildingTemplate = Instantiate(buildingPrefab1, ObjectSnapper.SnapToGridCell(hit.point, GridConstants.Instance.FloatCellSize()), Quaternion.Euler(0, 0, 0));
            isAStructureSelected = true;
        }
        else if (Input.GetKey(KeyCode.C))
        {
            Destroy(buildingTemplate);
            buildingTemplate = Instantiate(buildingPrefab2, ObjectSnapper.SnapToGridCell(hit.point, GridConstants.Instance.FloatCellSize()), Quaternion.Euler(0, 0, 0));
            isAStructureSelected = true;
        }
        else if (Input.GetKey(KeyCode.Escape))
            {
                Destroy(buildingTemplate);
                isAStructureSelected = false;
            }
        if (isAStructureSelected == true)
        {
            isSpaceOccupied = structureGrid.IsGridCellFilled(hit.point, buildingTemplate.GetComponent<Renderer>().bounds.size);
           
            if (!isSpaceOccupied)
                buildingTemplate.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
            else
            {
                buildingTemplate.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            }
            buildingTemplate.transform.position = ObjectSnapper.SnapToGridCell(hit.point, GridConstants.Instance.FloatCellSize(), buildingTemplate.GetComponent<Renderer>().bounds.size);
        }

        if (Input.GetMouseButtonDown(0) && isAStructureSelected == true && !isSpaceOccupied)
        {
            Destroy(buildingTemplate);
            isAStructureSelected = false;
            structureGrid.AddStructure(hit.point, buildingTemplate.GetComponent<Renderer>().bounds.size);
            Debug.Log(hit.point);
        }
    }

    private void DrawStructureOnGridOnClick()
    {

    }

}
