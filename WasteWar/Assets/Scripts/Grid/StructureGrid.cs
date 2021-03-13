using UnityEngine;


public class StructureGrid
{
    public float CellSize { get; private set; }
    public struct GridCoords
    {
        public int x { get; private set; }
        public int y { get; private set; }

        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

    }

    private int[,] Structures { get; set; }
   

    public StructureGrid(Terrain terrain, float cellSizeInInspector)
    {
        this.CellSize = cellSizeInInspector;
        Vector3 terrainPos; terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.GetComponent<Terrain>().terrainData.size;
        Structures = new int[(int)(terrainSize.x / CellSize), (int)(terrainSize.z / CellSize)];
    }

    public void AddStructure(Vector3 pos)
    {
        GridCoords gridPos = new GridCoords();
        gridPos = GetNearestCellOnGrid(pos);
        Structures[gridPos.x, gridPos.y] = 1;
    }

    private GridCoords GetNearestCellOnGrid(Vector3 pos)
    {
        int x= Mathf.RoundToInt(pos.x / CellSize);
        //int y = Mathf.RoundToInt(pos.y / CellSize); for the future
        int z = Mathf.RoundToInt(pos.z / CellSize);

        GridCoords gridPos = new GridCoords();
        gridPos.Set(x, z);

        return gridPos;

    }
}
