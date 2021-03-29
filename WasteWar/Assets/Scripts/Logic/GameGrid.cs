using UnityEngine;
using System.Collections.Generic;

//edge case, when  we put a big building at the top/right/top-right edge (for self reference)
public class GameGrid : MonoBehaviour
{
    private float CellSize { get; set; }

    private int[,] GridCoordinates { get; set; }

    private Dictionary<int, Structure> Structures;
    private Dictionary<int, Resource> Resources;

    private void Start()
    {
        GameEvents.StructurePlacedListeners += AddStructure;
    }

    public GameGrid(float xSize, float zSize, float cellSizeInInspector)
    {
        this.CellSize = cellSizeInInspector;
        GridCoordinates = new int[(int)(xSize / CellSize), (int)(zSize / CellSize)];
        Structures = new Dictionary<int, Structure>();
        Resources = new Dictionary<int, Resource>();
    }

    public void AddStructure(object sender,Vector3 pos, Vector3 buildingSize, int structureType)
    {
        // how many fields along X[] or Z[] the building takes 
        int XGridSize;
        int YGridSize;

        XYSize(out XGridSize,out YGridSize, buildingSize);

        GridCoords gridPos = GetNearestCellOnGrid(pos);    

        for (int i = 0; i < XGridSize; i++)
            for (int j = 0; j < YGridSize; j++)
                GridCoordinates[gridPos.X + i, gridPos.Y + j] = 1;
    }

    //TODO work on edge case when placing a structure at the edge of the map (might aswell just make edge unplayable area)
    public bool IsGridCellFilled(Vector3 pos, Vector3 buildingSize)
    {
        int XGridSize;
        int YGridSize;

        XYSize(out XGridSize,out YGridSize, buildingSize);
        GridCoords gridPos = GetNearestCellOnGrid(pos);

        for (int i = 0; i < XGridSize; i++)
            for (int j = 0; j < YGridSize; j++)
                if (GridCoordinates[gridPos.X + i , gridPos.Y + j]==1)
                    return true;
        return false;
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
    //get the x/z building span (how many cells in a given direction the building takes)
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

