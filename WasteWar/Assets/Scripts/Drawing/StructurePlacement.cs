using System.Collections.Generic;
using UnityEngine;
using Constants;

public class StructurePlacement : MonoBehaviour
{
    [SerializeField]
    private GameObject structurePrefab1;
    [SerializeField]
    private GameObject structurePrefab2;
    [SerializeField]
    private Terrain terrain;
    
    private RaycastHit hit;
    private Ray ray;

    private StructureGrid structureGrid;
    private GameObject structureTemplate;
    private Vector3 terrainSize;
    private Vector3 sizeOfStructureTemplate;

    private readonly List<GameObject>  Structures = new List<GameObject>();

    private bool isAStructureSelected = false;
    bool isSpaceOccupied = false;

    void Start()
    {
        terrainSize = terrain.GetComponent<Terrain>().terrainData.size;
        structureGrid = new StructureGrid(terrainSize.x,terrainSize.z, GridConstants.Instance.FloatCellSize());
    }

    void Update() {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
        PrepareTemplateStructure();

        if (isAStructureSelected == true)
        {
            sizeOfStructureTemplate = structureTemplate.GetComponent<Renderer>().bounds.size;

            isSpaceOccupied = structureGrid.IsGridCellFilled(hit.point, sizeOfStructureTemplate);
           
            if (!isSpaceOccupied)
                structureTemplate.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
            else
            {
                structureTemplate.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            }
            structureTemplate.transform.position = ObjectSnapper.SnapToGridCell(hit.point, GridConstants.Instance.FloatCellSize(), sizeOfStructureTemplate);
        }

        if (Input.GetMouseButtonDown(0) && isAStructureSelected == true && !isSpaceOccupied)
        {
            Destroy(structureTemplate);
            isAStructureSelected = false;
            structureGrid.AddStructure(hit.point, sizeOfStructureTemplate);
            DrawStructureOnGrid(structureTemplate,sizeOfStructureTemplate);
            Debug.Log(hit.point);
        }
    }

    private void DrawStructureOnGrid(GameObject structureToAdd,Vector3 structureSize)
    {
        structureToAdd.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        Structures.Add(Instantiate(structureToAdd, ObjectSnapper.SnapToGridCell(
            hit.point, 
            GridConstants.Instance.FloatCellSize(),structureSize), 
            Quaternion.Euler(0, 0, 0)
            ));
    }

    private void PrepareTemplateStructure()
    {
        foreach (var key in GameKeys.Instance.StructureKeybinds) {
            if (Input.GetKey(key))
            {
                switch (key)
                {
                    case KeyCode.B:
                        destroyPreviousAndPrepareNew(structurePrefab1);
                        break;
                    case KeyCode.C:
                        destroyPreviousAndPrepareNew(structurePrefab2);
                        break;
                    case KeyCode.Escape:
                        isAStructureSelected = false;
                        break;

                }
            }
        }
        if(isAStructureSelected)
        {
            structureTemplate = Instantiate(structureTemplate, ObjectSnapper.SnapToGridCell(hit.point, GridConstants.Instance.FloatCellSize()), Quaternion.Euler(0, 0, 0));
            isAStructureSelected = false;
        }
        void destroyPreviousAndPrepareNew(GameObject structure)
        {
            Destroy(structureTemplate);
            structureTemplate = structure;
            isAStructureSelected = true;
        }
    }
}
