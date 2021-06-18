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

    private NativeList<float3> gridLocations = new NativeList<float3>(Allocator.Persistent);
    private NativeList<ushort> gridBestCosts = new NativeList<ushort>(Allocator.Persistent);
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
        CreateBestCostList();
    }

    protected override void OnUpdate()
    {
        var gridLocs = gridLocations;
        var gridBest = gridBestCosts;
        var gridWidth = GridData.GetLength(0);
        Entities
           .WithAll<FlowFieldAgentComponent>()
           .WithReadOnly(gridLocs)
           .WithReadOnly(gridBest)
           .ForEach(
               (ref FlowFieldAgentComponent agent, ref Translation translation) =>
               {
                   agent.currentDestination = GetNextCellDestination(translation.Value, gridLocs,
                                    gridBest, gridWidth);
               }
           )
           .ScheduleParallel();
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
                        gridLocations.Add(GridData[x, z].centerPos);
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
            List<GridCell> curNeighbors = GetNeighborCells(currCell.gridIndex.x, currCell.gridIndex.y,false);
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

    public void CreateBestCostList()
    {
        foreach (var cell in GridData)
        {
            gridBestCosts.Add(cell.bestCost);
        }
    }

    public static float3 GetNextCellDestination(float3 origin, NativeList<float3> gridCellPositions,
        NativeList<ushort> bestCosts, int gridWidth)
    {
        float lowestDistanceFromGrid = 50000;
        int closestIndex = 0;
        for (int i = 0; i < gridCellPositions.Length; i++)
        {
            float currDistance = math.length(gridCellPositions[i] - origin);
            if (currDistance < lowestDistanceFromGrid)
            {
                lowestDistanceFromGrid = currDistance;
                closestIndex = i;
            }
        }

        float lowestNeighborCost = ushort.MaxValue;
        float3 closestNeighbor = gridCellPositions[0];
        ushort currentBestCost = bestCosts[closestIndex];

        int indexToCheck = closestIndex + 1;
        if (indexToCheck < bestCosts.Length  - 1 && bestCosts[indexToCheck] < lowestNeighborCost)
        {
            closestNeighbor = gridCellPositions[indexToCheck]; // EAST
            lowestNeighborCost = bestCosts[indexToCheck];
        }
        indexToCheck = closestIndex - 1;
        if (indexToCheck >= 0 && bestCosts[indexToCheck] < lowestNeighborCost)
        {
            closestNeighbor = gridCellPositions[indexToCheck]; // WEST
            lowestNeighborCost = bestCosts[indexToCheck];
        }
        indexToCheck = closestIndex + gridWidth;
        if (indexToCheck < bestCosts.Length - 1 && bestCosts[indexToCheck] < lowestNeighborCost)
        {
            closestNeighbor = gridCellPositions[indexToCheck]; // NORTH
            lowestNeighborCost = bestCosts[indexToCheck];
        }
        indexToCheck = closestIndex - gridWidth;
        if (indexToCheck >= 0 && bestCosts[indexToCheck] < lowestNeighborCost)
        {
            closestNeighbor = gridCellPositions[indexToCheck]; // SOUTH
            lowestNeighborCost = bestCosts[indexToCheck];
        }
        indexToCheck = closestIndex + gridWidth + 1;
        if (indexToCheck < bestCosts.Length - 1 &&
                bestCosts[indexToCheck] < lowestNeighborCost)
        {
            closestNeighbor = gridCellPositions[indexToCheck]; // NE
            lowestNeighborCost = bestCosts[indexToCheck];

        }
        indexToCheck = closestIndex + gridWidth - 1;
        if (indexToCheck < bestCosts.Length - 1 &&
                bestCosts[indexToCheck] < lowestNeighborCost)
        {
            closestNeighbor = gridCellPositions[indexToCheck]; // NW
            lowestNeighborCost = bestCosts[indexToCheck];

        }
        indexToCheck = closestIndex - gridWidth  + 1;

        if (indexToCheck >= 0 && bestCosts[indexToCheck] < lowestNeighborCost)
        {
            closestNeighbor = gridCellPositions[indexToCheck]; // SE
            lowestNeighborCost = bestCosts[indexToCheck];

        }
        indexToCheck = closestIndex - gridWidth - 1;

        if (indexToCheck >= 0 && bestCosts[indexToCheck] < lowestNeighborCost)
        {
            closestNeighbor = gridCellPositions[indexToCheck]; // SW
            lowestNeighborCost = bestCosts[indexToCheck];
        }
        return closestNeighbor;
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
    protected override void OnDestroy()
    {
        base.OnDestroy();
        gridLocations.Dispose();
        gridBestCosts.Dispose();
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
