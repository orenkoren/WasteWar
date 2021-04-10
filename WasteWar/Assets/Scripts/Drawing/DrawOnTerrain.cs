using Constants;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnTerrain : MonoBehaviour
{
    private const int KEY_GENERATOR = 10000;
    private const int CELL_TEXTURE_WIDTH = 6;
    private const int CELL_TEXTURE_HEIGHT = 6;
    private const int TEXTURE_LAYER_COUNT = 2;

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Terrain terrain;

    public GameObject TemplateStructure { get; set; }
    public Vector3 TemplateStructureSize { get; private set; }

    private RaycastHit hit;
    private Ray ray;
    private ResourceGrid Resources;
    private List<GameObject> Structures = new List<GameObject>();

    void Start()
    {
        Resources = new ResourceGrid(terrain.terrainData.size);

        GameEvents.TemplateSelectedListeners += DestroyPreviousAndPrepareNewTemplate;
        GameEvents.NodeUsedUpListeners += UpdateTerrainTexture;
        GameEvents.LeftClickPressedListeners += DrawStructure;
        GameEvents.RightClickPressedListeners += DeleteStructure;
        GameEvents.MouseOverListeners += Resources.ShowCurrentResourceAmount;

        DrawTerrainTexture();
        foreach (var resource in Resources.Nodes)
            DrawResourceTextureOnTerrain((float)(resource.Key / KEY_GENERATOR), (float)(resource.Key % KEY_GENERATOR));
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
        if (data.StructureType != StructureType.NONE && CheckIfLocationIsFree())
        {
            TemplateStructure.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            TemplateStructure.layer = LayerMasks.Instance.ATTACKABLE_LAYER;
            var Structure = Instantiate(TemplateStructure, ObjectSnapper.SnapToGridCell(
                data.mousePos,
                GridConstants.Instance.FloatCellSize(), TemplateStructureSize),
                Quaternion.Euler(GameConstants.Instance.DEFAULT_OBJECT_ROTATION)
                );

            if (data.StructureType == StructureType.BUILDING)
            {
                Structure.GetComponent<BuildingData>().IsTemplate = false;
                Structure.GetComponent<BuildingData>().Resources = Resources;
            }
            Structures.Add(Structure);
            Destroy(TemplateStructure);
            data.StructureType = StructureType.NONE;
        }
    }
    private void DeleteStructure(object sender, RaycastHit data)
    {
        Structures.Remove(data.collider.gameObject);
        Destroy(data.collider.gameObject);
    }

    private void SetTemplateStructureColor(Color color)
    {
        TemplateStructure.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }

    private void SetTemplateStructurePos(Vector3 pos)
    {
        TemplateStructure.transform.position = ObjectSnapper.SnapToGridCell(pos, GridConstants.Instance.FloatCellSize(), TemplateStructureSize);
    }

    private void DestroyPreviousAndPrepareNewTemplate(object sender, TemplateData data)
    {
        if (data.StructureType != StructureType.NONE)
        {
            Destroy(TemplateStructure);
            TemplateStructure = Instantiate(data.TemplateStructure,
                                ObjectSnapper.SnapToGridCell(data.mousePos, GridConstants.Instance.FloatCellSize()),
                                Quaternion.Euler(GameConstants.Instance.DEFAULT_OBJECT_ROTATION));
            TemplateStructure.layer = LayerMasks.Instance.IGNORE_RAYCAST_LAYER;
            TemplateStructureSize = TemplateStructure.GetComponent<Renderer>().bounds.size;
        }
        else
            Destroy(TemplateStructure);
    }

    private bool CheckIfLocationIsFree()
    {
        return !Physics.CheckBox(TemplateStructure.GetComponent<Collider>().bounds.center, TemplateStructure.GetComponent<Collider>().bounds.extents,
            TemplateStructure.transform.rotation, LayerMasks.Instance.ATTACKABLE);
    }

    private void DrawResourceTextureOnTerrain(float x, float z)
    {
        int textureWidth = CELL_TEXTURE_HEIGHT;
        int textureHeight = CELL_TEXTURE_WIDTH;

        GridUtils.GridCoords start =MapTextureCoordToWorldCoord(x, z);
        TexturePlacer(start, new GridUtils.GridCoords(textureWidth, textureHeight),false);
    }

    //has to be called on every run, because otherwise the terrain texture doesn't reset
    private void DrawTerrainTexture()
    {
        int textureWidth = 512;
        int textureHeight = 512;

        TexturePlacer(new GridUtils.GridCoords(0, 0), new GridUtils.GridCoords(textureWidth, textureHeight),true);
    }
    private void UpdateTerrainTexture(object sender, int locationKey)
    {
        int x = locationKey / KEY_GENERATOR;
        int y = locationKey % KEY_GENERATOR;
        int textureWidth = CELL_TEXTURE_HEIGHT;
        int textureHeight = CELL_TEXTURE_WIDTH;

        GridUtils.GridCoords start = MapTextureCoordToWorldCoord(x, y);
        TexturePlacer(start, new GridUtils.GridCoords(textureWidth, textureHeight), true);
    }

    private void TexturePlacer(GridUtils.GridCoords start,GridUtils.GridCoords dimensions,bool resource)
    {
        //arg 1 height, arg 2 width, arg3 layer count
        float[,,] splatMapData = new float[dimensions.X, dimensions.Y, TEXTURE_LAYER_COUNT];

        for (int i = 0; i < dimensions.X; i++)
        {
            for (int j = 0; j < dimensions.Y; j++)
            {//layer 0 opacity 0% layer 1 opacity 100%
                splatMapData[i, j, 0] = resource ? 1 : 0;
                splatMapData[i, j, 1] = resource ? 0 : 1;
            }
        }
        terrain.terrainData.SetAlphamaps(start.X, start.Y, splatMapData);
    }

    private GridUtils.GridCoords MapTextureCoordToWorldCoord(float x, float z)
    {
        //mapping texture coords to world terrain coords
        int mapX = (int)((x / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth);
        int mapY = (int)((z / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight);

        return new GridUtils.GridCoords(mapX, mapY);
    }

    private void OnDestroy()
    {  // why unsubscribe here and not inside ResourceGrid destructor??
        GameEvents.TemplateSelectedListeners -= DestroyPreviousAndPrepareNewTemplate;
        GameEvents.NodeUsedUpListeners -= UpdateTerrainTexture;
        GameEvents.LeftClickPressedListeners -= DrawStructure;
        GameEvents.RightClickPressedListeners -= DeleteStructure;
        GameEvents.MouseOverListeners -= Resources.ShowCurrentResourceAmount;
    }
}
