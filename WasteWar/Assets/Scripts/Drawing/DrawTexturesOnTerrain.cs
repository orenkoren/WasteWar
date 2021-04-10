using System.Collections.Generic;
using UnityEngine;

public class DrawTexturesOnTerrain : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;

    private const int KEY_GENERATOR = 10000;
    private const int CELL_TEXTURE_WIDTH = 6;
    private const int CELL_TEXTURE_HEIGHT = 6;
    private const int TEXTURE_LAYER_COUNT = 2;

    private void Start()
    {
        DrawTerrainTexture();
        GameEvents.NodeUsedUpListeners += UpdateTerrainTexture;
        GameEvents.LoadingTerrainTexturesListeners += DrawAllResourceTexturesOnTerrain;
    }

    private void DrawAllResourceTexturesOnTerrain(object sender, ResourceGrid resources)
    {
        foreach (var resource in resources.Nodes)
            DrawResourceTextureOnTerrain((float)(resource.Key / KEY_GENERATOR), (float)(resource.Key % KEY_GENERATOR));
    }

    private void DrawResourceTextureOnTerrain(float x, float z)
    {
        int textureWidth = CELL_TEXTURE_HEIGHT;
        int textureHeight = CELL_TEXTURE_WIDTH;

        GridUtils.GridCoords start = MapTextureCoordToWorldCoord(x, z);
        TexturePlacer(start, new GridUtils.GridCoords(textureWidth, textureHeight), false);
    }

    //has to be called on every run, because otherwise the terrain texture doesn't reset
    private void DrawTerrainTexture()
    {
        int textureWidth = 512;
        int textureHeight = 512;

        TexturePlacer(new GridUtils.GridCoords(0, 0), new GridUtils.GridCoords(textureWidth, textureHeight), true);
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

    private void OnDestroy()
    {
        GameEvents.NodeUsedUpListeners -= UpdateTerrainTexture;
    }
}
