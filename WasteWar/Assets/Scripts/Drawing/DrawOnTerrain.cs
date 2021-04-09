using Constants;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnTerrain : MonoBehaviour
{
    private const int KEY_GENERATOR = 10000;

    private const int TEXTURE_WIDTH = 6;

    private const int TEXTURE_HEIGHT = 6;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private Terrain terrain;

    public GameObject TemplateStructure { get; set; }
    public Vector3 TemplateStructureSize { get; private set; }
    private RaycastHit hit;
    private Ray ray;
    private ResourceGrid Resources;
    private readonly List<GameObject> Structures = new List<GameObject>();

    void Start()
    {

        GameEvents.TemplateSelectedListeners += DestroyPreviousAndPrepareNewTemplate;
        GameEvents.StructurePlacedListeners += DrawStructure;

        drawTerrainTexture();
        Resources = new ResourceGrid(terrain.terrainData.size);
        foreach (var resource in Resources.Resources)
            drawResourceTextures((float)(resource.Key / KEY_GENERATOR), (float)(resource.Key % KEY_GENERATOR));
    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
            && TemplateStructure != null && MathUtilBasic.CursorIsWithinBounds(hit.point, terrain.terrainData.size))
            DrawTemplateStructureAt(hit.point);
    }

    public void DrawTemplateStructureAt(Vector3 loc)
    {
        if (checkIfLocationIsFree())
            SetTemplateStructureColor(Color.green);
        else
            SetTemplateStructureColor(Color.red);
        SetTemplateStructurePos(loc);
    }

    //TODO fix placement near edges of the map
    public void DrawStructure(object sender, TemplateData data)
    {
        if (data.StructureType != StructureType.NONE && checkIfLocationIsFree())
        {
            TemplateStructure.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            TemplateStructure.layer = LayerMasks.Instance.ATTACKABLE_LAYER;
            Structures.Add(Instantiate(TemplateStructure, ObjectSnapper.SnapToGridCell(
                data.mousePos,
                GridConstants.Instance.FloatCellSize(), TemplateStructureSize),
                Quaternion.Euler(GameConstants.Instance.DEFAULT_OBJECT_ROTATION)
                ));
            Destroy(TemplateStructure);
            data.StructureType = StructureType.NONE;
        }
    }

    public void SetTemplateStructureColor(Color color)
    {
        TemplateStructure.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }

    public void SetTemplateStructurePos(Vector3 pos)
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

    private bool checkIfLocationIsFree()
    {
        return !Physics.CheckBox(TemplateStructure.GetComponent<Collider>().bounds.center, TemplateStructure.GetComponent<Collider>().bounds.extents,
            TemplateStructure.transform.rotation, LayerMasks.Instance.ATTACKABLE);
    }

    private void drawResourceTextures(float x, float z)
    {
        var terrainSize = terrain.GetComponent<Terrain>().terrainData.size;

        //mapping texture coords to world coords
        int mapX = (int)((x / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth);
        int mapZ = (int)((z / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight);

        //arg 1 height, arg 2 width, arg3 layer count
        float[,,] splatMapData = new float[TEXTURE_WIDTH, TEXTURE_HEIGHT, 2];

        for (int i = 0; i < TEXTURE_WIDTH; i++)
        {
            for (int j = 0; j < TEXTURE_HEIGHT; j++)
            {
                splatMapData[i, j, 0] = 0;
                splatMapData[i, j, 1] = 1;
            }
        }
        terrain.terrainData.SetAlphamaps(mapX, mapZ, splatMapData);
    }

    //has to be called on every run, because otherwise the terrain texture doesn't reset
    private void drawTerrainTexture()
    {
        float[,,] splatMapData = new float[512, 512, 2];

        for (int i = 0; i < 512; i++)
        {
            for (int j = 0; j < 512; j++)
            {
                splatMapData[i, j, 0] = 1;
                splatMapData[i, j, 1] = 0;
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, splatMapData);
    }

    private void OnDestroy()
    {
        GameEvents.TemplateSelectedListeners -= DestroyPreviousAndPrepareNewTemplate;
        GameEvents.StructurePlacedListeners -= DrawStructure;
    }
}
