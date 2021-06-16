using Constants;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(EnemySpawnerSystem))]
public class EnemyPatternSystem : SystemBase
{
    private bool shouldSkipAFrame = true;
    private GridSystem gridSystem;
    private NativeList<float3> positionsArray = new NativeList<float3>(Allocator.Persistent);


    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnemyPatternSystemEnabler>();
        gridSystem = World.GetOrCreateSystem<GridSystem>();
    }

    protected override void OnUpdate()
    {
        if (shouldSkipAFrame)
        {
            shouldSkipAFrame = false;
            return;
        }
        EntityManager.DestroyEntity(GetSingletonEntity<EnemyPatternSystemEnabler>());
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
            SpawnZAttack(playerBasePosition, random, gridSystem);
        if (spawnerComponent.pattern == SpawnPattern.Square)
            SpawnSquare(playerBasePosition, random);
        if (spawnerComponent.pattern == SpawnPattern.Asterix)
            SpawnAsterix(playerBasePosition, random);
        if (spawnerComponent.pattern == SpawnPattern.Focused)
            SpawnFocused(playerBasePosition, random);
        shouldSkipAFrame = true;
    }

    // Another approach for the spawn algorithms is to use entityInQueryIndex in regards to spawnAmount ( index % spawnAmount is position)
    private void SpawnZAttack(Translation playerBasePosition, Random random, GridSystem grid)
    {
        var posArray = positionsArray;
        //TODO: implemnet function in GridSystem: GetAdjacentPositionsByAmount(int startPos, int amount)
        Entities
            .WithoutBurst()
            .WithAll<AttackerComponent>()
            .ForEach((int entityInQueryIndex) =>
            {
                UnityEngine.Debug.Log(entityInQueryIndex);
                posArray.Add(grid.GetGridPosition(entityInQueryIndex, (int)random.NextFloat(900, 999)));
            }).Run();

        Entities
            .WithAll<AttackerComponent>()
            .ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation) =>
            {
                //var spawnLocation = new float3(random.NextFloat(0, 1000), 5, random.NextFloat(900, 1000));
                float3 spawnLocation = posArray[entityInQueryIndex];
                translation.Value = spawnLocation;
                rotation.Value = quaternion.LookRotation(
                    new float3(playerBasePosition.Value.x, 0, playerBasePosition.Value.z) - spawnLocation, math.up());
            }).ScheduleParallel();
    }

    private void SpawnSquare(Translation playerBasePosition, Random random)
    {
        Entities
                  .WithAll<AttackerComponent>()
                  .ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation) =>
                  {
                      float3 spawnLocation;
                      var spawnPlace = random.NextFloat(0, 1);
                      if (spawnPlace > 0.75f)
                          spawnLocation = new float3(20, 3, random.NextFloat(20, 980));
                      else if (spawnPlace > 0.5f)
                          spawnLocation = new float3(980, 3, random.NextFloat(20, 980));
                      else if (spawnPlace > 0.25f)
                          spawnLocation = new float3(random.NextFloat(20, 980), 3, 980);
                      else
                          spawnLocation = new float3(random.NextFloat(20, 980), 3, 20);


                      translation.Value = spawnLocation;
                      rotation.Value = quaternion.LookRotation(
                          new float3(playerBasePosition.Value.x, 0, playerBasePosition.Value.z) - spawnLocation, math.up());
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
    }
}
