using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[assembly: RegisterGenericComponentType(typeof(List<>))]
public class GridSystem : SystemBase
{
    public GridCell[,] GridData = new GridCell[0, 0];
    private BuildPhysicsWorld m_physicsWorld;
    private GridCell destinationCell;
    private int cellSize;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
        CreateGrid();
        CreateCostField();
        CreateIntegrationField(GridData[25, 25]);
    }

    protected override void OnUpdate()
    {
        var gridLocations = GridData;
        VisualizeGrid(gridLocations);
        Entities
           .WithoutBurst()
           .WithAll<FlowFieldAgentComponent>()
           .WithReadOnly(gridLocations)
           .ForEach(
               (ref FlowFieldAgentComponent agent, ref Translation translation) =>
               {
                   agent.currentDestination = GetNextCellDestination(translation.Value,
                       agent.finalDestination);
               }
           )
           .Run();
    }

    private void CreateGrid()
    {
        Entities
            .WithoutBurst()
            .WithAll<GridComponent>()
            .ForEach((ref GridComponent grid, in Entity e) =>
            {
                GridData = new GridCell[grid.Width / grid.CellSize + 1, grid.Height / grid.CellSize + 1];
                cellSize = grid.CellSize;
                for (int x = 0; x < GridData.GetLength(0); x++)
                {
                    for (int z = 0; z < GridData.GetLength(1); z++)
                    {
                        GridData[x, z] = new GridCell
                        {
                            bottomLeftPos = new float3(x * grid.CellSize, 0, z * grid.CellSize),
                            centerPos = new float3(x * grid.CellSize + (grid.CellSize / 2), 0,
                                                z * grid.CellSize + (grid.CellSize / 2)),
                            gridIndex = new int2(x, z),
                            cost = 1
                        };
                    }
                }
                EntityManager.GetComponentObject<ECSGridGUI>(e).grid = GridData;
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

    public void CreateIntegrationField(GridCell destination)
    {
        destinationCell = destination;
        destinationCell.cost = 0;
        destinationCell.bestCost = 0;

        Queue<GridCell> cellsToCheck = new Queue<GridCell>();
        cellsToCheck.Enqueue(destinationCell);

        while (cellsToCheck.Count > 0)
        {
            GridCell currCell = cellsToCheck.Dequeue();
            List<GridCell> curNeighbors = GetNeighborCells(currCell.gridIndex.x, currCell.gridIndex.y, false);
            foreach (var curNeighbor in curNeighbors)
            {
                if (curNeighbor.cost == byte.MaxValue) { continue; }
                if (curNeighbor.cost + currCell.bestCost < curNeighbor.bestCost)
                {
                    curNeighbor.bestCost = (ushort)(curNeighbor.cost + currCell.bestCost);
                    cellsToCheck.Enqueue(curNeighbor);
                }
            }
        }
    }

    public float3 GetNextCellDestination(float3 origin, float3 finalDestination)
    {
        float lowestDistanceFromGrid = 50000;
        GridCell closestToGrid = GridData[0, 0];
        foreach (var cell in GridData)
        {
            float currDistance = math.length(cell.centerPos - origin);
            if (currDistance < lowestDistanceFromGrid)
            {
                lowestDistanceFromGrid = currDistance;
                closestToGrid = cell;
            }
        }
        float lowestNeighborCost = ushort.MaxValue;
        GridCell closestNeighbor = GridData[0, 0];
        var neighbors = GetNeighborCells(closestToGrid.gridIndex.x, closestToGrid.gridIndex.y, true);
        foreach (var neighbor in neighbors)
        {
            if(neighbor.bestCost < lowestNeighborCost)
            {
                lowestNeighborCost = neighbor.bestCost;
                closestNeighbor = neighbor;
            }
        }
        return closestNeighbor.centerPos;
    }

    private List<GridCell> GetNeighborCells(int xIndex, int zIndex, bool shouldCheckDiagonals)
    {
        var cellList = new List<GridCell>();
        var xLength = GridData.GetLength(0);
        var zLength = GridData.GetLength(1);
        if (zIndex != zLength - 1)
            cellList.Add(GridData[xIndex, zIndex + 1]); // north
        if (xIndex != xLength - 1)
            cellList.Add(GridData[xIndex + 1, zIndex]); // east
        if (zIndex != 0)
            cellList.Add(GridData[xIndex, zIndex - 1]); // south
        if (xIndex != 0)
            cellList.Add(GridData[xIndex - 1, zIndex]); // west
        if (xIndex != xLength - 1 && zIndex != zLength - 1 && shouldCheckDiagonals)
            cellList.Add(GridData[xIndex + 1, zIndex + 1]);  //NE
        if (xIndex != xLength - 1 && zIndex != 0 && shouldCheckDiagonals)
            cellList.Add(GridData[xIndex + 1, zIndex - 1]);  // SE
        if (xIndex != 0 && zIndex != 0 && shouldCheckDiagonals)
            cellList.Add(GridData[xIndex - 1, zIndex - 1]);  //SW
        if (xIndex != 0 && zIndex != zLength - 1 && shouldCheckDiagonals)
            cellList.Add(GridData[xIndex - 1, zIndex + 1]);  // NW

        return cellList;
    }

    private void VisualizeGrid(GridCell[,] gridLocations)
    {
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
}

public class GridCell
{
    public float3 bottomLeftPos;
    public float3 centerPos;
    public int2 gridIndex;
    public byte cost;
    public ushort bestCost = ushort.MaxValue;

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
