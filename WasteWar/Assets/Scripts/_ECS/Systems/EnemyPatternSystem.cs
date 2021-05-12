using Constants;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(EnemySpawnerSystem))]
public class EnemyPatternSystem : SystemBase
{
    private bool shouldSkipAFrame = true;
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnemyPatternSystemEnabler>();
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
            SpawnZAttack(playerBasePosition, random);
        if (spawnerComponent.pattern == SpawnPattern.Square)
            SpawnSquare(playerBasePosition, random);
        shouldSkipAFrame = true;
    }

    private void SpawnZAttack(Translation playerBasePosition, Random random)
    {
        Entities
                  .WithAll<AttackerComponent>()
                  .ForEach((int entityInQueryIndex, Entity e, ref Translation translation, ref Rotation rotation) =>
                  {
                      var spawnLocation = new float3(random.NextFloat(0, 1000), 1, random.NextFloat(900, 1000));
                      translation.Value = spawnLocation;
                      rotation.Value = quaternion.LookRotation(
                          new float3(playerBasePosition.Value.x, 0, playerBasePosition.Value.z) - spawnLocation, math.up());
                  }).ScheduleParallel();
    }

    private void SpawnSquare(Translation playerBasePosition, Random random)
    {
        Entities
                  .WithAll<AttackerComponent>()
                  .ForEach((int entityInQueryIndex, Entity e, ref Translation translation, ref Rotation rotation) =>
                  {
                      float3 spawnLocation;
                      var spawnPlace = random.NextFloat(0, 1);
                      if (spawnPlace > 0.75f)
                          spawnLocation = new float3(20, 1, random.NextFloat(20, 980));
                      else if (spawnPlace > 0.5f)
                          spawnLocation = new float3(980, 1, random.NextFloat(20, 980));
                      else if (spawnPlace > 0.25f)
                          spawnLocation = new float3(random.NextFloat(20, 980), 1, 980);
                      else
                          spawnLocation = new float3(random.NextFloat(20, 980), 1, 20);


                      translation.Value = spawnLocation;
                      rotation.Value = quaternion.LookRotation(
                          new float3(playerBasePosition.Value.x, 0, playerBasePosition.Value.z) - spawnLocation, math.up());
                  }).ScheduleParallel();
    }
}
