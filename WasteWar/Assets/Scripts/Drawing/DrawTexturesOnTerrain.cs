using UnityEngine;

/// <summary>
/// work in progress not used rn
/// </summary>

public class DrawTexturesOnTerrain : MonoBehaviour
{
    private Terrain terrain;
    [SerializeField]
    private RuntimeGameObjRefs runtimeGameObjRefs;

    private const int TEXTURE_LAYER_COUNT = 2;

    private void Start()
    {
        terrain = runtimeGameObjRefs.terrain;
    }

    private void DrawAllResourceTexturesOnTerrain(object sender, ResourceGrid resources)
    {
        DrawTerrainTexture();
    }

    private void DrawResourceTextureOnTerrain(float x, float z)
    {
        int textureWidth = 1;
        int textureHeight = 1;

        GridUtils.GridCoords start = MapTextureCoordToWorldCoord(x, z);
        TexturePlacer(start, new GridUtils.GridCoords(textureWidth, textureHeight), false);
    }

    //has to be called on every run, because otherwise the terrain texture doesn't reset
    //IMPORTANT alphamap size needs to be a multiple of 2^x
    private void DrawTerrainTexture()
    {
        TexturePlacer(new GridUtils.GridCoords(0, 0), 
                      new GridUtils.GridCoords(terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight), true);
    }

    private void TexturePlacer(GridUtils.GridCoords start, GridUtils.GridCoords dimensions, bool resource)
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

}
