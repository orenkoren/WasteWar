using UnityEngine;

public class DrawStateManager : MonoBehaviour
{
    [SerializeField]
    private GameObject structurePrefab1;
    [SerializeField]
    private GameObject structurePrefab2;
    [SerializeField]
    private Terrain terrain;
    [SerializeField]
    private Camera cam;

    private RaycastHit hit;
    private Ray ray;
    private Vector3 terrainSize;

    private void Start()
    {
        terrainSize = terrain.terrainData.size;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
            && CursorIsWithinBounds(hit.point))
            {
                GameEvents.FireDrawStateChanged(this, new DrawData { StructurePrefab = structurePrefab1, StructureLocation = hit.point });
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameEvents.FireDrawStateChanged(this, null);
        }
    }

    private bool CursorIsWithinBounds(Vector3 hitLocation)
    {
        int failedConditions = 0;
        //TODO unnecessary to go through all 1, should short circuit if any condition is true
        failedConditions = hitLocation.x > terrainSize.z ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.x < 0 ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.z > terrainSize.z ? (failedConditions + 1) : failedConditions;
        failedConditions = hitLocation.z < 0 ? (failedConditions + 1) : failedConditions;
        print(failedConditions);
        return failedConditions == 0;
    }
}
