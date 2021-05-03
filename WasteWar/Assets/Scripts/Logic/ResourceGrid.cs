using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class ResourceGrid
{
    private const float STOPPING_PROBABILITY_BLOCK = 0.01f;
    private const float STOPPING_PROBABILITY_MAP = 0.5f;
    private const int MIN_NODES = 50;

    public Dictionary<int, Resource> Nodes { get; private set; } = new Dictionary<int, Resource>();

    private GridUtils.GridCoords gridSize;

    public ResourceGrid(Vector3 terrainSize)
    {
        gridSize = GridUtils.Vector3ToGridCoord(terrainSize);
        GenerateResourceNodes();
    }

    private void GenerateResourceNodes()
    {
        do
        {
            int x = Random.Range((int)CameraConstants.Instance.WORLD_BORDER, gridSize.X - (int)CameraConstants.Instance.WORLD_BORDER);
            int y = Random.Range((int)CameraConstants.Instance.WORLD_BORDER, gridSize.Y - (int)CameraConstants.Instance.WORLD_BORDER);
            GenerateOneBlockOfResourcesAt(new GridUtils.GridCoords(x, y));
        }
        while ((Random.Range(0f, 1f) < (1 - STOPPING_PROBABILITY_MAP)));
    }

    private void GenerateOneBlockOfResourcesAt(GridUtils.GridCoords loc)
    {
        HashSet<GridUtils.GridCoords> available = new HashSet<GridUtils.GridCoords>();
        available.Add(loc);
        int i = 0;
        
        AddCoal(loc.X,loc.Y);
        AddNeighbours(available, loc.X,loc.Y);

        while (Random.Range(0f, 1f) < (1 - STOPPING_PROBABILITY_BLOCK) || i < MIN_NODES)
        {
            int x=0;
            int y=0;

            var index = Random.Range(0, available.Count);

            int j = 0;
            // TODO refactor, unnecessarily looping til reaching the element, use a different data structure or smth
            foreach (var element in available)
            {
                if (j == index)
                {
                    x = element.X;
                    y = element.Y;
                    break;
                }
                j++;
            }

            AddCoal(x, y);
            AddNeighbours(available, x, y);
            available.Remove(new GridUtils.GridCoords(x, y));
            i++;
        }
    }


    private void AddCoal(int x, int y)
    {
        if (!Nodes.ContainsKey((x) * GridConstants.Instance.CELL_COUNT + y))
            Nodes.Add(x * GridConstants.Instance.CELL_COUNT + y, new Coal());
    }

    private void AddNeighbours(HashSet<GridUtils.GridCoords> available, int x, int y)
    {
        if (!Nodes.ContainsKey((x + 1) * GridConstants.Instance.CELL_COUNT + y) && checkXYValidity(x + 1, y))
        {
            available.Add(new GridUtils.GridCoords(x + 1, y));
        }
        if (!Nodes.ContainsKey((x - 1) * GridConstants.Instance.CELL_COUNT + y) && checkXYValidity(x - 1, y))
        {
            available.Add(new GridUtils.GridCoords(x - 1, y));
        }
        if (!Nodes.ContainsKey(x * GridConstants.Instance.CELL_COUNT + (y + 1)) && checkXYValidity(x, y + 1))
        {
            available.Add(new GridUtils.GridCoords(x, y + 1));
        }
        if (!Nodes.ContainsKey(x * GridConstants.Instance.CELL_COUNT + (y - 1)) && checkXYValidity(x, y - 1))
        {
            available.Add(new GridUtils.GridCoords(x, y - 1));
        }
    }

    private bool checkXYValidity(int x, int y)
    {
        return ((x >= (int)CameraConstants.Instance.WORLD_BORDER && x <= gridSize.X - (int)CameraConstants.Instance.WORLD_BORDER) && 
            (y >= (int)CameraConstants.Instance.WORLD_BORDER && y <= gridSize.Y - (int)CameraConstants.Instance.WORLD_BORDER));
    }
    //TODO refactor, improve by only checking if mouse is over a resource, also how to unsubscribe without making this public
    public void ShowCurrentResourceAmount(object sender,Vector3 hitPoint)
    {
        var gridCoord = GridUtils.Vector3ToGridCoord(hitPoint);
        int key = gridCoord.X * GridConstants.Instance.CELL_COUNT + gridCoord.Y;

        //if (Nodes.ContainsKey(key))
        //    Debug.Log(Nodes[key].Count);
    }
}
