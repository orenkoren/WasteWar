using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public class GridSystem : SystemBase
{
    public GridCellData[,] GridData = new GridCellData[0, 0];
    private BuildPhysicsWorld m_physicsWorld;
    private int cellSize;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
        CreateGrid();
        CreateCostField();
    }

    protected override void OnUpdate()
    {
        var gridLocations = GridData;
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
                            if (gridLocations[x, z].cost == 255)
                            {
                                Debug.DrawLine(gridLocations[x, z].bottomLeftPos, gridLocations[x + 1, z].bottomLeftPos);
                                Debug.DrawLine(gridLocations[x, z].bottomLeftPos, gridLocations[x, z + 1].bottomLeftPos);
                                Debug.DrawLine(gridLocations[x + 1, z].bottomLeftPos, gridLocations[x + 1, z + 1].bottomLeftPos);
                                Debug.DrawLine(gridLocations[x, z + 1].bottomLeftPos, gridLocations[x + 1, z + 1].bottomLeftPos);
                            }
                        }
                    }
                }
            }).Run();
    }

    public float3 GetGridPosition(int x, int z)
    {
        return GridData[x, z].bottomLeftPos;
    }

    private void CreateGrid()
    {
        Entities
            .WithoutBurst()
            .WithAll<GridComponent>()
            .ForEach((ref GridComponent grid) =>
            {
                GridData = new GridCellData[grid.Width / grid.CellSize + 1, grid.Height / grid.CellSize + 1];
                cellSize = grid.CellSize;
                for (int x = 0; x < GridData.GetLength(0); x++)
                {
                    for (int z = 0; z < GridData.GetLength(1); z++)
                    {
                        GridData[x, z] = new GridCellData
                        {
                            bottomLeftPos = new float3(x * grid.CellSize, 0, z * grid.CellSize),
                            centerPos = new float3(x * grid.CellSize + (grid.CellSize / 2), 0,
                                                z * grid.CellSize + (grid.CellSize / 2)),
                            gridIndex = new float2(x, z),
                            cost = 1
                        };
                    }
                }
            }).Run();
    }

    private void CreateCostField()
    {
        CollisionWorld pworld = m_physicsWorld.PhysicsWorld.CollisionWorld;
        float3 cellHalfExtends = new float3(cellSize / 2, 1, cellSize / 2);
        var collisionFilterImpassable = new CollisionFilter()
        {
            BelongsTo = ~0u,
            CollidesWith = 1 << 22,
            GroupIndex = 0
        };
        foreach (var cell in GridData)
        {
            NativeList<DistanceHit> outHits = new NativeList<DistanceHit>(Allocator.Temp);
            pworld.OverlapBox(cell.centerPos, quaternion.identity, cellHalfExtends,
                                ref outHits, collisionFilterImpassable);
            if (outHits.Length > 0)
                cell.IncreaseCost(255);
            outHits.Dispose();
        }
    }
}

public class GridCellData
{
    public float3 bottomLeftPos;
    public float3 centerPos;
    public float2 gridIndex;
    public byte cost;

    public void IncreaseCost(int amount)
    {
        if (cost == byte.MaxValue)
            return;
        if (cost + amount >= byte.MaxValue)
            cost = byte.MaxValue;
        else
            cost += (byte)amount;
    }
}
