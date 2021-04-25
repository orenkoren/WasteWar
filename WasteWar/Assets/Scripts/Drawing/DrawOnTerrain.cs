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
    private GameObject PipeMethods;

    public GameObject TemplateStructure { get; set; }
    public Vector3 TemplateStructureSize { get; private set; }

    private RaycastHit hit;
    private Ray ray;
    private ResourceGrid Resources;
    private List<GameObject> Structures = new List<GameObject>();

    private void Start()
    {
        Resources = new ResourceGrid(terrain.terrainData.size);

        GameEvents.TemplateSelectedListeners += DestroyOldAndCreateNewTemplate;
        GameEvents.LeftClickPressedListeners += DrawStructure;
        GameEvents.RightClickPressedListeners += DeleteStructure;
        GameEvents.MouseOverListeners += Resources.ShowCurrentResourceAmount;
        GameEvents.BuildingRotationListeners += RotateTemplate;

        GameEvents.FireLoadingTerrainTextures(this, Resources);
    }

    private void Update()
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
            if (data.TemplateStructure.tag.Contains("Pipe"))
            {
                TemplateInstantiator(PipeMethods.GetComponent<PipeLogic>().CheckNeighbors(TemplateStructure), data.MousePos);
                GameEvents.FirePipePlaced2(this, TemplateStructure.tag);
            }

            TemplateStructure.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            TemplateStructure.layer = LayerMasks.Instance.ATTACKABLE_LAYER;

            GameObject Structure = null;
            TemplateInstantiator(data.MousePos, ref Structure);

            if (Structure.tag.Contains("Pipe"))
                GameEvents.FirePipePlaced(this, Structure);

            if (data.TemplateStructure.CompareTag("Building"))
            {
                Structure.GetComponent<BuildingData>().IsTemplate = false;
                Structure.GetComponent<BuildingData>().Resources = Resources;
            }

            Structures.Add(Structure);
            Destroy(TemplateStructure);
            data.TemplateStructure = null;
        }
    }

    private void DeleteStructure(object sender, RaycastHit data)
    {
        Structures.Remove(data.collider.gameObject);
        Destroy(data.collider.gameObject);
    }

    private void DestroyOldAndCreateNewTemplate(object sender, TemplateData data)
    {
        if (data.TemplateStructure != null)
        {
            TemplateInstantiator(data.TemplateStructure, data.MousePos);
        }
        else
            Destroy(TemplateStructure);
    }

    private void TemplateInstantiator(GameObject Template, Vector3 mousePos)
    {
        Destroy(TemplateStructure);
        TemplateStructure = Instantiate(Template,
                            ObjectSnapper.SnapToGridCell(mousePos),
                            Template.transform.rotation);
        TemplateStructure.layer = LayerMasks.Instance.IGNORE_RAYCAST_LAYER;
        TemplateStructureSize = TemplateStructure.GetComponent<Renderer>().bounds.size;
    }

    private void TemplateInstantiator(Vector3 mousePos, ref GameObject StructureBeingAssignedTo)
    {
        //TODO quickhack until the model is fixed (it goes out of bounds of the 1,1,1 cube,model needs to fit the square basically)
        if (TemplateStructure.tag.Contains("Pipe") && TemplateStructure.tag.Length > 4)
            TemplateStructureSize = new Vector3(1f, 1f, 1f);

        StructureBeingAssignedTo = Instantiate(TemplateStructure,
                            ObjectSnapper.SnapToGridCell(mousePos, TemplateStructureSize),
                            TemplateStructure.transform.rotation);
        TemplateStructure.layer = LayerMasks.Instance.IGNORE_RAYCAST_LAYER;
        TemplateStructureSize = TemplateStructure.GetComponent<Renderer>().bounds.size;

        Destroy(TemplateStructure);
    }

    private void RotateTemplate(object sender, int i)
    {
        if (TemplateStructure)
        {
            //TODO quick hack, refactor this
            var pos = TemplateStructure.transform.position;
            var temp = TemplateStructure.GetComponent<PipeState>();
            if (temp != null)
            {
                if (TemplateStructure.CompareTag("PipeTB"))
                {
                    TemplateInstantiator(PipeMethods.GetComponent<PipeLogic>().pipes.LeftRight, pos);
                }

                else if (TemplateStructure.CompareTag("PipeLR"))
                {
                    TemplateInstantiator(PipeMethods.GetComponent<PipeLogic>().pipes.TopBottom, pos);
                }
            }
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
    {
        GameEvents.BuildingRotationListeners -= RotateTemplate;
        GameEvents.TemplateSelectedListeners -= DestroyOldAndCreateNewTemplate;
        GameEvents.LeftClickPressedListeners -= DrawStructure;
        GameEvents.RightClickPressedListeners -= DeleteStructure;
        GameEvents.MouseOverListeners -= Resources.ShowCurrentResourceAmount;
    }
}
