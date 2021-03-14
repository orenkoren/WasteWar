using UnityEngine;

public class StructurePlacement : MonoBehaviour
{
    [SerializeField]
    private GameObject buildingPrefab;
    [SerializeField]
    private Terrain terrain;
    //not a primitive value? so why out?
    
    private RaycastHit hit;
    private Ray ray;

    private Vector3 terrainSize;
    private StructureGrid structureGrid;
    private GameObject buildingTemplate;
    private bool isStructureSelected = false;

    void Start()
    {
        terrainSize = terrain.GetComponent<Terrain>().terrainData.size;
        structureGrid = new StructureGrid(terrainSize.x,terrainSize.z, GridConstants.Instance.FloatCellSize());
    }

    void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && CursorIsWithinBounds(hit.point))
        {

            if (Input.GetKey(KeyCode.B) && isStructureSelected == false)
            {
                buildingTemplate= Instantiate(buildingPrefab, ObjectSnapper.SnapToGridCell(hit.point, GridConstants.Instance.FloatCellSize()), Quaternion.Euler(0, 0, 0));
                isStructureSelected = true;
            }
            if (Input.GetKey(KeyCode.Escape) && isStructureSelected == true)
            {
                Destroy(buildingTemplate);
                isStructureSelected = false;
            }

            if (Input.GetMouseButtonDown(0) && isStructureSelected == true)
            {
                Destroy(buildingTemplate);
                isStructureSelected = false;
                structureGrid.AddStructure(hit.point);
                Debug.Log(hit.point);
            }

            if (isStructureSelected == true)
                buildingTemplate.transform.position = ObjectSnapper.SnapToGridCell(hit.point, GridConstants.Instance.FloatCellSize(),buildingTemplate.GetComponent<Renderer>().bounds.size);
        }
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
}