using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(EnemySpawnerSystem))]
public class EnemyPatternSystem : SystemBase
{
    EntityCommandBufferSystem m_ecb;
    private bool shouldSkipAFrame = true;

    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnemyPatternSystemEnabler>();
    }

    protected override void OnUpdate()
    {
        if (!shouldSkipAFrame)
            EntityManager.DestroyEntity(GetSingletonEntity<EnemyPatternSystemEnabler>());
        shouldSkipAFrame = false;
        m_ecb = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        var ecb = m_ecb.CreateCommandBuffer().AsParallelWriter();
        Entities
          .WithAll<AttackerComponent>()
          .ForEach((int entityInQueryIndex, Entity e) =>
          {
              ecb.SetComponent(entityInQueryIndex, e, new Translation
              {
                  Value = new float3(450, 0, 450)
              });
          }).ScheduleParallel();

        m_ecb.AddJobHandleForProducer(Dependency);

    }
}
