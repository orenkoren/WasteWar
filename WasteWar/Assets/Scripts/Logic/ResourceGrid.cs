using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

[CreateAssetMenu(fileName ="Resource Grid", menuName ="Scriptable Objects")]
public class ResourceGrid : ScriptableObject
{
    private const float STOPPING_PROBABILITY = 0.01f;
    private const int MIN_RESOURCES = 50;
    private const int KEY_GENERATOR = 10000;


    private Dictionary<int, Resource> Resources = new Dictionary<int, Resource>();

    public Dictionary<int,Resource> GenerateResources(Vector3 terrainSize)
    {
        GridCoords gridSize = Vector3ToGridCoord(terrainSize);
        int x= Random.Range(0, gridSize.X- (int)CameraConstants.Instance.WORLD_BORDER);
        int y = Random.Range(0, gridSize.Y - (int)CameraConstants.Instance.WORLD_BORDER); ; 
        int i = 0;
        HashSet<GridCoords> available = new HashSet<GridCoords>();

        AddCoal(x, y);
        AddNeighbours(available,x, y);

        while (Random.Range(0f, 1f) < 1 - STOPPING_PROBABILITY || i < MIN_RESOURCES)
        {
            var index = Random.Range(0, available.Count);

            int j = 0;
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
            AddNeighbours(available,x, y);
            available.Remove(new GridCoords(x, y));
            i++;
        }
        return Resources;
    }

    private void AddCoal(int x, int y)
    {
        Resources.Add(x * KEY_GENERATOR + y, new Coal());
    }


    private void AddNeighbours(HashSet<GridCoords> available,int x,int y)
{
        if (!Resources.ContainsKey( (x + 1) * KEY_GENERATOR + y  ))
        {
            available.Add(new GridCoords(x + 1 ,y));
        }
        if (!Resources.ContainsKey( (x - 1) * KEY_GENERATOR + y ))
        {
            available.Add(new GridCoords(x - 1, y));
        }
        if (!Resources.ContainsKey( x * KEY_GENERATOR + (y + 1) ))
        {
            available.Add(new GridCoords(x, y + 1));
        }
        if (!Resources.ContainsKey( x * KEY_GENERATOR + (y - 1) ))
        {
            available.Add(new GridCoords(x, y - 1));
        }
    }

    private GridCoords Vector3ToGridCoord(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int z = Mathf.FloorToInt(pos.z);

        GridCoords gridPos = new GridCoords(x, z);

        return gridPos;
    }
}

public struct GridCoords
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public GridCoords(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}

