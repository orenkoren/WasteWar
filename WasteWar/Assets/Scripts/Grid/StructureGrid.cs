﻿using UnityEngine;


public class StructureGrid
{
    private float CellSize { get;set; }
    // placeholder array type, will change to game object instances
    private int[,] Structures { get; set; }

    public StructureGrid(float xSize, float zSize, float cellSizeInInspector)
    {
        this.CellSize = cellSizeInInspector;
        Structures = new int[(int)(xSize / CellSize), (int)(zSize / CellSize)];
    }

    public void AddStructure(Vector3 pos,Vector3 size)
    {
        GridCoords gridPos = GetNearestCellOnGrid(pos);
        Structures[gridPos.X, gridPos.Y] = 1;
        Debug.Log(gridPos.X.ToString()+' '+gridPos.Y.ToString());
    }

    private GridCoords GetNearestCellOnGrid(Vector3 pos)
    {
        int x= Mathf.FloorToInt(pos.x / CellSize);
        //int y = Mathf.RoundToInt(pos.y / CellSize); for the future
        int z = Mathf.FloorToInt(pos.z / CellSize);

        GridCoords gridPos = new GridCoords();
        gridPos.Set(x, z);

        return gridPos;
    }
    public bool isGridCellFilled(Vector3 pos)
    {
        GridCoords gridPos = GetNearestCellOnGrid(pos);
        return Structures[gridPos.X, gridPos.Y] != 1;
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