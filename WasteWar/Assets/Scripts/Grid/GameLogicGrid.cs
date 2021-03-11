using UnityEngine;


public class GameLogicGrid
{
    private readonly Terrain terrain;
    private Mesh cells = new Mesh();
    public float CellSize { get; } = 1f;
    public int[,] Elements { get; set; }
    Vector3 terrainPos;
    Vector3 terrainSize;

    public GameLogicGrid(Terrain terrain)
    {
        this.terrain = terrain;
        terrainPos = terrain.transform.position;
        terrainSize = terrain.GetComponent<Terrain>().terrainData.size;
        Elements = new int[(int)(terrainSize.x / CellSize), (int)(terrainSize.z / CellSize)];
    }

      
    //    cells.SetIndices(, MeshTopology.Lines);

        
    //}
    //public Vector3 GetNearestPointOnGrid(Vector3 position)
    //{
    //    int xCount = Mathf.RoundToInt(position.x / CellSize);
    //    int yCount = Mathf.RoundToInt(position.y / CellSize);
    //    int zCount = Mathf.RoundToInt(position.z / CellSize);

    //    return new Vector3((float)xCount * CellSize, (float)yCount * CellSize, (float)zCount * CellSize);

    //}
    //public Vector3 GridSize { get; set; }


}