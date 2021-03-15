using UnityEngine;

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

        if (Physics.Raycast(ray, out hit, 100f, LayerMasks.GROUND) && CursorIsWithinBounds(hit.point))
            PlaceStructureInGridOnClick();
    }
    private bool CursorIsWithinBounds(Vector3 hitLocation)
    {
        int failedConditions = 0;
        failedConditions = hitLocation.x > terrainSize.z ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.x < 0 ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.z > terrainSize.z ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.z < 0 ? (failedConditions + 1) : failedConditions;

        return failedConditions == 0;
    }
    private void PlaceStructureInGridOnClick()
    {
    //will create some kind of switch case for this later on
        if (Input.GetKey(KeyCode.B) && isAStructureSelected == false)
        {
            buildingTemplate = Instantiate(buildingPrefab1, ObjectSnapper.SnapToGridCell(hit.point, GridConstants.Instance.FloatCellSize()), Quaternion.Euler(0, 0, 0));
            isAStructureSelected = true;
        }
        if (Input.GetKey(KeyCode.C) && isAStructureSelected == false)
        {
            buildingTemplate = Instantiate(buildingPrefab2, ObjectSnapper.SnapToGridCell(hit.point, GridConstants.Instance.FloatCellSize()), Quaternion.Euler(0, 0, 0));
            isAStructureSelected = true;
        }
        if (Input.GetKey(KeyCode.Escape) && isAStructureSelected == true)
            {
                Destroy(buildingTemplate);
                isAStructureSelected = false;
            }
        if (Input.GetMouseButtonDown(0) && isAStructureSelected == true)
        {
            Destroy(buildingTemplate);
            isAStructureSelected = false;
            structureGrid.AddStructure(hit.point, buildingTemplate.GetComponent<Renderer>().bounds.size);
            Debug.Log(hit.point);
        }
        if (isAStructureSelected == true)
        {
            if (structureGrid.isGridCellFilled(hit.point))
                buildingTemplate.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
            else
            {
                buildingTemplate.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            }
            buildingTemplate.transform.position = ObjectSnapper.SnapToGridCell(hit.point, GridConstants.Instance.FloatCellSize(), buildingTemplate.GetComponent<Renderer>().bounds.size);
        }
    }
}
