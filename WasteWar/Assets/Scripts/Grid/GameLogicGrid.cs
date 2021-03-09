using UnityEngine;

public class GameLogicGrid
{
    private readonly GameObject terrain;
    private Mesh cells = new Mesh();
    private readonly float cellSize=1f;
    private int width;
    private int height;
    public int[,] elements { get; set; }

    public GameLogicGrid()
    {
        terrain = GameObject.FindWithTag("Terrain");
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.GetComponent<Terrain>().terrainData.size;

       elements = new int[(int)(terrainSize.x/cellSize),(int) (terrainSize.z/cellSize)];
        cells.SetIndices(, MeshTopology.Lines);

        
    }
    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        int xCount = Mathf.RoundToInt(position.x / cellSize);
        int yCount = Mathf.RoundToInt(position.y / cellSize);
        int zCount = Mathf.RoundToInt(position.z / cellSize);

        return new Vector3((float)xCount * cellSize, (float)yCount * cellSize, (float)zCount * cellSize);

    }
    public Vector3 GridSize { get; set; }
    public float CellSize { get; }

}