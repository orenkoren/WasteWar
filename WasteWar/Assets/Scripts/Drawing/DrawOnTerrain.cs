using Constants;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnTerrain : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Terrain terrain;
    [SerializeField]
    private Prefabs Pipes;

    public GameObject TemplateStructure { get; set; }
    public Vector3 TemplateStructureSize { get; private set; }

    private RaycastHit hit;
    private Ray ray;
    private ResourceGrid Resources;
    private List<Pipeline> pipelines = new List<Pipeline>();
    private List<GameObject> Structures = new List<GameObject>();

    void Start()
    {
        Resources = new ResourceGrid(terrain.terrainData.size);

        GameEvents.TemplateSelectedListeners += DestroyPreviousAndPrepareNewTemplate;
        GameEvents.LeftClickPressedListeners += DrawStructure;
        GameEvents.RightClickPressedListeners += DeleteStructure;
        GameEvents.MouseOverListeners += Resources.ShowCurrentResourceAmount;
        GameEvents.BuildingRotationListeners += RotateTemplate90Deg;

        GameEvents.FireLoadingTerrainTextures(this, Resources);
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
            && TemplateStructure != null && MathUtils.CursorIsWithinBounds(hit.point, terrain.terrainData.size))
            DrawTemplateStructureAt(hit.point);
    }

    public void DrawTemplateStructureAt(Vector3 loc)
    {
        if (CheckIfLocationIsFree())
            SetTemplateStructureColor(Color.green);
        else
            SetTemplateStructureColor(Color.red);
        SetTemplateStructurePos(loc);
    }

    //TODO fix placement near edges of the map
    private void DrawStructure(object sender, TemplateData data)
    {
        if (data.TemplateStructure != null && CheckIfLocationIsFree())
        {
            Vector3 gridPosition = ObjectSnapper.SnapToGridCell(data.mousePos, TemplateStructureSize);

            if (data.TemplateStructure.tag.Contains("Pipe"))
            {
                DestroyPreviousAndCreatePipeTemplate(TemplateStructure.GetComponent<PipeState>().CheckNeighbors(Pipes), gridPosition);
            }

            TemplateStructure.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            TemplateStructure.layer = LayerMasks.Instance.ATTACKABLE_LAYER;

            var Structure = Instantiate(TemplateStructure,
                    gridPosition,
                    TemplateStructure.transform.rotation);
            if (data.TemplateStructure.tag == "Building")
            {
                Structure.GetComponent<BuildingData>().IsTemplate = false;
                Structure.GetComponent<BuildingData>().Resources = Resources;
            }
            Structures.Add(Structure);

            Destroy(TemplateStructure);
            data.TemplateStructure= null;
        }
    }
    private void DeleteStructure(object sender, RaycastHit data)
    {
        Structures.Remove(data.collider.gameObject);
        Destroy(data.collider.gameObject);
    }

    private void DestroyPreviousAndPrepareNewTemplate(object sender, TemplateData data)
    {
        if (data.TemplateStructure != null)
        {
            Destroy(TemplateStructure);
            TemplateStructure = Instantiate(data.TemplateStructure,
                                ObjectSnapper.SnapToGridCell(data.mousePos),
                                data.TemplateStructure.transform.rotation);
            TemplateStructure.layer = LayerMasks.Instance.IGNORE_RAYCAST_LAYER;
            TemplateStructureSize = TemplateStructure.GetComponent<Renderer>().bounds.size;
        }
        else
            Destroy(TemplateStructure);
    }
    private void DestroyPreviousAndCreatePipeTemplate(GameObject gameObject, Vector3 gridPosition)
    {
        Destroy(TemplateStructure);
        TemplateStructure = Instantiate(gameObject,
                            gridPosition,
                            gameObject.transform.rotation);
        TemplateStructure.layer = LayerMasks.Instance.IGNORE_RAYCAST_LAYER;
        TemplateStructureSize = TemplateStructure.GetComponent<Renderer>().bounds.size;
    }


    private void RotateTemplate90Deg(object sender, int i)
    {
        if (TemplateStructure)
        {
            TemplateStructure.transform.Rotate(0f, 90f, 0f, Space.World);
            var temp = TemplateStructure.GetComponent<PipeState>();
            if (temp != null)
                temp.Rotate();
        }
    }

    private void SetTemplateStructureColor(Color color)
    {
        TemplateStructure.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }

    private void SetTemplateStructurePos(Vector3 pos)
    {
        TemplateStructure.transform.position = ObjectSnapper.SnapToGridCell(pos, GridConstants.Instance.FloatCellSize(), TemplateStructureSize);
    }

    private bool CheckIfLocationIsFree()
    {
        return !Physics.CheckBox(TemplateStructure.GetComponent<Collider>().bounds.center, TemplateStructure.GetComponent<Collider>().bounds.extents,
            TemplateStructure.transform.rotation, LayerMasks.Instance.ATTACKABLE);
    }

    private void OnDestroy()
    {  // why unsubscribe here and not inside ResourceGrid destructor??
        GameEvents.BuildingRotationListeners -= RotateTemplate90Deg;
        GameEvents.TemplateSelectedListeners -= DestroyPreviousAndPrepareNewTemplate;
        GameEvents.LeftClickPressedListeners -= DrawStructure;
        GameEvents.RightClickPressedListeners -= DeleteStructure;
        GameEvents.MouseOverListeners -= Resources.ShowCurrentResourceAmount;
    }
}
