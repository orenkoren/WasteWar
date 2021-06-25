using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(EnemySpawnerSystem))]
[UpdateAfter(typeof(GridSystem))]
public class EnemyPatternSystem : SystemBase
{
    private bool shouldSkipAFrame = true;
    private GridSystem gridSystem;
    private NativeList<float3> positionsArray = new NativeList<float3>(Allocator.Persistent);
    private NativeList<float3> verts = new NativeList<float3>(Allocator.Persistent);

    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnemyPatternSystemEnabler>();
        RequireSingletonForUpdate<GridSystemFinishedBuilding>();
        gridSystem = World.GetOrCreateSystem<GridSystem>();
    }

    protected override void OnUpdate()
    {
        if (shouldSkipAFrame)
        {
            shouldSkipAFrame = false;
            return;
        }

        var gridWidth = gridSystem.GridData.GetLength(0);
        var cellSize = gridSystem.cellSize;
        var gridBestCosts = gridSystem.gridBestCosts;
        EntityManager.DestroyEntity(GetSingletonEntity<EnemyPatternSystemEnabler>());
        EntityManager.DestroyEntity(GetSingletonEntity<GridSystemFinishedBuilding>());
        var playerBasePosition = GameConstants.Instance.PlayerBasePosition;
        EnemySpawnerComponent spawnerComponent = new EnemySpawnerComponent();
        Entities
            .WithAll<EnemySpawnerComponent>()
            .ForEach((in EnemySpawnerComponent spawner) =>
            {
                spawnerComponent = spawner;
            }).Run();
        Random random = new Random(56);
        if (spawnerComponent.pattern == SpawnPattern.Zattack)
            SpawnZAttack(playerBasePosition);
        if (spawnerComponent.pattern == SpawnPattern.Square)
            SpawnSquare(playerBasePosition, random, spawnerComponent.spawnAmount);
        if (spawnerComponent.pattern == SpawnPattern.Asterix)
            SpawnAsterix(playerBasePosition, random);
        if (spawnerComponent.pattern == SpawnPattern.Focused)
            SpawnFocused(playerBasePosition, random);
        if (spawnerComponent.pattern == SpawnPattern.Sword)
            SpawnSword(playerBasePosition, spawnerComponent.spawnAmount);
        shouldSkipAFrame = true;
        EntityManager.CreateEntity(typeof(EnemyPatternFinished));
    }

    private void SpawnSword(Translation playerBasePosition, int totalAmount)
    {
        Entities
            .WithoutBurst()
            .WithAll<SwordComponent>()
            .ForEach((in Entity e) =>
            {
                var vertList = EntityManager.GetComponentObject<GetVertices>(e).vertices;
                foreach (var vert in vertList)
                {
                    verts.Add(vert);
                }
            }).Run();
        var vertices = verts;
        var skipAmount = 1;
        if (vertices.Length > totalAmount)
            skipAmount = vertices.Length / totalAmount;
        Entities
            .WithAll<AttackerComponent>()
            .WithReadOnly(vertices)
            .ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation) =>
            {
                float3 spawnLocation = vertices[entityInQueryIndex % vertices.Length * skipAmount];
                translation.Value = spawnLocation;
                rotation.Value = quaternion.LookRotation(
                    new float3(playerBasePosition.Value.x, 0, playerBasePosition.Value.z) - spawnLocation, math.up());
            }).ScheduleParallel();
    }

    private void SpawnZAttack(Translation playerBasePosition)
    {
        float xSpacing = 10;
        float zSpacing = 30;
        float spawnHeight = 150;
        float startZPos = 1500;

        Entities
            .WithAll<AttackerComponent>()
            .ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation) =>
            {
                var spawnLocation = new float3(entityInQueryIndex % 100 * xSpacing, spawnHeight,
                                        startZPos + entityInQueryIndex / 100 * zSpacing);
                translation.Value = spawnLocation;
                rotation.Value = quaternion.LookRotation(
                    new float3(playerBasePosition.Value.x, 0, playerBasePosition.Value.z) - spawnLocation, math.up());
            }).ScheduleParallel();
    }

    private void SpawnSquare(Translation playerBasePosition, Random random, int totalAmount)
    {
        float startX = -1500;
        float startZ = -1500;
        float spawnHeight = 200;
        float2 line1Start = new float2(startX, startZ);
        float2 line1End = new float2(startX, -startZ);
        float2 line2Start = line1End;
        float2 line2End = new float2(-startX, -startZ);
        float2 line3Start = line2End;
        float2 line3End = new float2(-startX, startZ);
        float2 line4Start = line3End;
        float2 line4End = line1Start;
        float spacing = 10;
        Entities
                  .WithAll<AttackerComponent>()
                  .ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation) =>
                  {
                      float2 spawnLocation = float2.zero;
                      var determineRow = (float)entityInQueryIndex / totalAmount;
                      if (determineRow <= 0.25f)
                          spawnLocation = GetPosOnLine(entityInQueryIndex, line1Start, line1End, spacing);
                      else if (determineRow <= 0.5f)
                          spawnLocation = GetPosOnLine(entityInQueryIndex, line2Start, line2End, spacing);
                      else if (determineRow <= 0.75f)
                          spawnLocation = GetPosOnLine(entityInQueryIndex, line3Start, line3End, spacing);
                      else if (determineRow <= 1f)
                          spawnLocation = GetPosOnLine(entityInQueryIndex, line4Start, line4End, spacing);

                      float2 GetPosOnLine(int index, float2 start, float2 end, float spacing)
                      {
                          float2 direction = math.normalize(start - end);
                          float distance = index * spacing;
                          return direction * distance;
                      }
                      float3 spawnLocation3 = new float3(spawnLocation.x, spawnHeight, spawnLocation.y);
                      translation.Value = spawnLocation3;
                      rotation.Value = quaternion.LookRotation(
                          new float3(playerBasePosition.Value.x, 0, playerBasePosition.Value.z) - spawnLocation3, math.up());
                  }).ScheduleParallel();
    }

    private void SpawnAsterix(Translation playerBasePosition, Random random)
    {
        Entities
                  .WithAll<AttackerComponent>()
                  .ForEach((ref Translation translation, ref Rotation rotation, ref AttackerComponent attacker) =>
                  {
                      float3 spawnLocation;
                      attacker.speed = random.NextFloat(attacker.speed * 0.5f, attacker.speed);
                      var spawnPlace = random.NextFloat(0, 1);
                      if (spawnPlace > 0.875f)
                          spawnLocation = new float3(20, 3, 980); // top left
                      else if (spawnPlace > 0.75f)
                          spawnLocation = new float3(500, 3, 980); // top middle
                      else if (spawnPlace > 0.625f)
                          spawnLocation = new float3(980, 3, 980); // top left
                      else if (spawnPlace > 0.5f)
                          spawnLocation = new float3(20, 3, 500); // mid left
                      else if (spawnPlace > 0.375f)
                          spawnLocation = new float3(980, 3, 500); // mid right
                      else if (spawnPlace > 0.25f)
                          spawnLocation = new float3(20, 3, 20); // bottom left
                      else if (spawnPlace > 0.125f)
                          spawnLocation = new float3(500, 3, 20); // bottom middle
                      else
                          spawnLocation = new float3(980, 3, 20); // bottom right

                      translation.Value = spawnLocation;
                      rotation.Value = quaternion.LookRotation(
                          new float3(playerBasePosition.Value.x, 0, playerBasePosition.Value.z) - spawnLocation, math.up());
                  }).ScheduleParallel();
    }

    private void SpawnFocused(Translation playerBasePosition, Random random)
    {
        Entities
                  .WithAll<AttackerComponent>()
                  .ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation) =>
                  {
                      var spawnLocation = new float3(500, 3, 950);
                      translation.Value = spawnLocation;
                      rotation.Value = quaternion.LookRotation(
                          new float3(playerBasePosition.Value.x, 0, playerBasePosition.Value.z) - spawnLocation, math.up());
                  }).ScheduleParallel();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        positionsArray.Dispose();
        verts.Dispose();
    }
}
