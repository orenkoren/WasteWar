using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class GridSystem : SystemBase
{
    public GridCell[,] GridData = new GridCell[0, 0];

    public NativeList<float2> gridLocations = new NativeList<float2>(Allocator.Persistent);
    public NativeList<ushort> gridBestCosts = new NativeList<ushort>(Allocator.Persistent);
    private NativeList<ushort> originalCosts = new NativeList<ushort>(Allocator.Persistent);
    public NativeList<bool> takenCells = new NativeList<bool>(Allocator.Persistent);
    public NativeList<int> costsToIncrease = new NativeList<int>(Allocator.Persistent);
    public int cellSize;
    private BuildPhysicsWorld m_physicsWorld;
    private GridCell destinationCell;
    private float resetTimer = 1f;
    private float currentTimer = 0f;


    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
        CreateGrid();
        CreateCostField();
        CreateIntegrationField(GridData[GridData.GetLength(0) / 2, GridData.GetLength(1) / 2]);
        CreateBestCostList();
        SetDefaultDestination(GridData[GridData.GetLength(0) / 2, GridData.GetLength(1) / 2].centerPos);
        EntityManager.CreateEntity(typeof(GridSystemFinishedBuilding));
    }

    protected override void OnUpdate()
    {
        var gridLocs = gridLocations;
        var gridBest = gridBestCosts;
        var writeCells = takenCells.AsParallelWriter();
        var readCells = takenCells.AsParallelReader();
        var taken = takenCells;
        var gridWidth = GridData.GetLength(0);
        var cellS = cellSize;
        Entities
           .WithAll<FlowFieldAgentComponent>()
           .WithReadOnly(gridBest)
           .WithReadOnly(gridLocs)
           .ForEach(
               (ref FlowFieldAgentComponent agent, in Translation translation, in Rotation rot) =>
               {
                   if (translation.Value.z <= 1000)
                   {
                       agent.currentDestination = GetNextCellDestination(ref agent,
                           gridLocs, gridBest, gridWidth, cellS, MathUtilECS.ToXZPlane(translation.Value), readCells);
                   }
               }
           )
           .ScheduleParallel();
        Entities
            .WithoutBurst()
           .WithAll<FlowFieldAgentComponent>()
           .ForEach(
               (ref FlowFieldAgentComponent agent) =>
               {
                   taken[agent.previousGridIndex] = false;
                   taken[agent.currentGridIndex] = true;
               }
           )
           .Run();

        //Entities
        //   .WithoutBurst()
        //   .WithAll<FlowFieldAgentComponent>()
        //   .ForEach(
        //       (ref FlowFieldAgentComponent agent) =>
        //       {
        //           var currIndex = agent.currentGridIndex;
        //           var prevIndex = agent.previousGridIndex;
        //           if (currIndex != prevIndex)
        //           {
        //               if (originalCosts[currIndex] != ushort.MaxValue)
        //                   gridBestCosts[currIndex] = 1000;
        //               gridBestCosts[prevIndex] = originalCosts[prevIndex];
        //           }
        //       }
        //   )
        //   .Run();
        currentTimer += Time.DeltaTime;
        if (currentTimer > resetTimer)
        {
            currentTimer = 0;
            //for (int i = 0; i < gridBestCosts.Length; i++)
            //{
            //    gridBestCosts[i] = originalCosts[i];
            //}
            for (int i = 0; i < takenCells.Length; i++)
            {
                takenCells[i] = false;
            }
        }
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
                            bottomLeftPos = new float2(x * grid.CellSize, z * grid.CellSize),
                            centerPos = new float2(x * grid.CellSize + (grid.CellSize / 2),
                                                z * grid.CellSize + (grid.CellSize / 2)),
                            gridIndex = new int2(x, z),
                            cost = 1
                        };
                        gridLocations.Add(GridData[x, z].centerPos);
                    }
                }
                EntityManager.GetComponentObject<ECSGridGUI>(e).grid = GridData;
                EntityManager.GetComponentObject<ECSGridGUI>(e).gridPositions = gridLocations;
                EntityManager.GetComponentObject<ECSGridGUI>(e).bestCosts = gridBestCosts;
                EntityManager.GetComponentObject<ECSGridGUI>(e).takenCells = takenCells;
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
            pworld.OverlapBox(new float3(cell.centerPos.x, 0, cell.centerPos.y),
                                    quaternion.identity, cellHalfExtends,
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
            List<GridCell> curNeighbors = GetNeighborCells(currCell.gridIndex.x, currCell.gridIndex.y, false, true);
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
            originalCosts.Add(cell.bestCost);
            takenCells.Add(false);
        }
    }

    public static float3 GetNextCellDestination(ref FlowFieldAgentComponent agent, NativeList<float2> gridCellPositions,
        NativeList<ushort> bestCosts, int gridWidth, int cellSize, float2 origin, NativeArray<bool>.ReadOnly readCells)
    {
        if (agent.shouldFlank)
        {
            var dist = math.distance(origin, MathUtilECS.ToXZPlane(agent.finalDestination));
            if (dist > 30 && !agent.flankReached)
                return agent.finalDestination;
            else
                agent.flankReached = true;
        }

        float lowestNeighborCost = ushort.MaxValue;
        var closestIndex = ((int)origin.x / cellSize * gridWidth) + ((int)origin.y / cellSize);
        if (closestIndex > readCells.Length)
            return agent.currentDestination;

        if (closestIndex != agent.currentGridIndex && closestIndex != agent.previousGridIndex)
        {
            agent.previousGridIndex = agent.currentGridIndex;
            agent.currentGridIndex = closestIndex;
        }

        int bestIndex = 0;
        bool shouldAvoid = agent.shouldAvoidCollisions;
        CheckIndex(closestIndex + 1); // N
        CheckIndex(closestIndex - 1); // S
        CheckIndex(closestIndex + gridWidth);  // E
        CheckIndex(closestIndex - gridWidth); // W
        if (!agent.IsRook)
        {
            CheckIndex(closestIndex + gridWidth + 1); // NE
            CheckIndex(closestIndex + gridWidth - 1); // NW
            CheckIndex(closestIndex - gridWidth + 1); // SE
            CheckIndex(closestIndex - gridWidth - 1); // SW
        }

        void CheckIndex(int indexToCheck)
        {
            if (indexToCheck < bestCosts.Length - 1 && indexToCheck >= 0)
            {
                var bestCost = bestCosts[indexToCheck];
                if (shouldAvoid)
                {
                    if (bestCost < lowestNeighborCost && readCells[indexToCheck] == false)
                    {
                        lowestNeighborCost = bestCost;
                        bestIndex = indexToCheck;
                    }
                }
                else if (bestCost < lowestNeighborCost)
                {
                    lowestNeighborCost = bestCost;
                    bestIndex = indexToCheck;
                }
            }
        }
        agent.nextGridIndex = bestIndex;
        if (bestIndex == 0)
            return agent.finalDestination;
        return new float3(gridCellPositions[bestIndex].x, 3, gridCellPositions[bestIndex].y);
    }

    private List<GridCell> GetNeighborCells(int xIndex, int zIndex, bool shouldCheckDiagonals, bool shouldCheckHorizontal)
    {
        var cellList = new List<GridCell>();
        var xLength = GridData.GetLength(0);
        var zLength = GridData.GetLength(1);
        if (zIndex != zLength - 1)
            cellList.Add(GridData[xIndex, zIndex + 1]); // north
        if (xIndex != xLength - 1 && shouldCheckHorizontal)
            cellList.Add(GridData[xIndex + 1, zIndex]); // east
        if (zIndex != 0)
            cellList.Add(GridData[xIndex, zIndex - 1]); // south
        if (xIndex != 0 && shouldCheckHorizontal)
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

    public void SetDefaultDestination(float2 dest)
    {
        //Entities
        //  .WithoutBurst()
        //  .WithAll<FlowFieldAgentComponent>()
        //  .ForEach(
        //      (ref FlowFieldAgentComponent agent, in Translation translation) =>
        //      {
        //          agent.currentDestination = new float3(dest.x, 20, dest.y);
        //          agent.finalDestination = new float3(dest.x, 20, dest.y);
        //      }
        //  )
        //  .Run();
    }


    public static ushort GetPositionCost(float2 pos, NativeList<ushort> bestCosts, int gridWidth, int cellSize)
    {
        return bestCosts[((int)pos.x / cellSize * gridWidth) + ((int)pos.y / cellSize)];
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        gridLocations.Dispose();
        gridBestCosts.Dispose();
        takenCells.Dispose();
        costsToIncrease.Dispose();
        originalCosts.Dispose();
    }
}

public class GridCell
{
    public float2 bottomLeftPos;
    public float2 centerPos;
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
