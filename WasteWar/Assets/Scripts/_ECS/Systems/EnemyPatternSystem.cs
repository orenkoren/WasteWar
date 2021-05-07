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
        int spawnAmount = 0;
        Entities
            .WithAll<EnemySpawnerComponent>()
            .ForEach((in EnemySpawnerComponent spawner) =>
            {
                spawnAmount = spawner.spawnAmount;
            }).Run();
        Random random = new Random(56);
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
}
