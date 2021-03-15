using UnityEngine;

//edge case, when  we put a big building at the top/right/top-right edge (for self reference)
public class StructureGrid
{
    private float CellSize { get; set; }
    // placeholder array type, will change to game object instances
    private int[,] Structures { get; set; }

    public StructureGrid(float xSize, float zSize, float cellSizeInInspector)
    {
        this.CellSize = cellSizeInInspector;
        Structures = new int[(int)(xSize / CellSize), (int)(zSize / CellSize)];
    }

    public void AddStructure(Vector3 pos, Vector3 buildingSize)
    {
        // how many fields along X[] or Z[] the building takes 
        int XGridSize;
        int YGridSize;

        XYSize(out XGridSize,out YGridSize, buildingSize);

        GridCoords gridPos = GetNearestCellOnGrid(pos);
        for (int i = 0; i < XGridSize; i++)
            for (int j = 0; j < YGridSize; j++)
            {
                Debug.Log((gridPos.X + i).ToString() + ' ' + (gridPos.X + i).ToString());
                Structures[gridPos.X + i, gridPos.Y + j] = 1;
            }
    }

    public bool IsGridCellFilled(Vector3 pos, Vector3 buildingSize)
    {
        int XGridSize;
        int YGridSize;

        XYSize(out XGridSize,out YGridSize, buildingSize);
        GridCoords gridPos = GetNearestCellOnGrid(pos);

        int failedConditions = 0;
        //unnecessary to go through all 1, should short circuit if any condition is true FIX LATER
        failedConditions = Structures[gridPos.X, gridPos.Y] == 1 ? (failedConditions + 1) : failedConditions;
        failedConditions = Structures[gridPos.X + XGridSize - 1, gridPos.Y] == 1 ? (failedConditions + 1) : failedConditions;
        failedConditions = Structures[gridPos.X , gridPos.Y + YGridSize - 1] == 1 ? (failedConditions + 1) : failedConditions;
        failedConditions = Structures[gridPos.X + XGridSize - 1, gridPos.Y + YGridSize - 1] == 1 ? (failedConditions + 1) : failedConditions;

        return failedConditions != 0;
    }

    private GridCoords GetNearestCellOnGrid(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / CellSize);
        //int y = Mathf.RoundToInt(pos.y / CellSize); for the future
        int z = Mathf.FloorToInt(pos.z / CellSize);

        GridCoords gridPos = new GridCoords();
        gridPos.Set(x, z);

        return gridPos;
    }

    private void XYSize(out int x , out int y,Vector3 buildingSize)
    {
        x = (int)(buildingSize.x / CellSize);
        y = (int)(buildingSize.z / CellSize);
    }
}

public struct GridCoords
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public void Set(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}
