using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GridSystem : SystemBase
{
    public float3[,] GridLocations = new float3[0, 0];

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        Entities
            .WithoutBurst()
            .WithAll<GridComponent>()
            .ForEach((ref GridComponent grid) =>
            {
                GridLocations = new float3[(grid.Width) / grid.CellSize + 1, grid.Height / grid.CellSize + 1];
                for (int x = 0; x < GridLocations.GetLength(0); x++)
                {
                    for (int z = 0; z < GridLocations.GetLength(1); z++)
                    {
                        GridLocations[x, z] = new float3(x * grid.CellSize, 0, z * grid.CellSize);
                    }
                }
            }).Run();
    }

    protected override void OnUpdate()
    {
        var gridLocations = GridLocations;
        Entities
            .WithoutBurst()
            .WithAll<GridComponent>()
            .ForEach((ref GridComponent grid) =>
            {
                if (grid.ShouldVisualize)
                {
                    for (int x = 0; x < gridLocations.GetLength(0) - 1; x++)
                    {
                        for (int z = 0; z < gridLocations.GetLength(1) - 1; z++)
                        {
                            Debug.DrawLine(gridLocations[x, z], gridLocations[x + 1, z]);
                            Debug.DrawLine(gridLocations[x, z], gridLocations[x, z + 1]);
                            Debug.DrawLine(gridLocations[x + 1, z], gridLocations[x + 1, z + 1]);
                            Debug.DrawLine(gridLocations[x, z + 1], gridLocations[x + 1, z + 1]);
                        }
                    }
                }
            }).Run();
    }

    public float3 GetGridPosition(int x, int z)
    {
        return GridLocations[x, z];
    }
}

public class GridCellData
{
    public float3 position;
    public int cost;
}
