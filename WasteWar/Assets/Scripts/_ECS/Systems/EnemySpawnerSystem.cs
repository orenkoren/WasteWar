using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

public class EnemySpawnerSystem : SystemBase
{
    private EnemySpawnerComponent spawner;
    private EntityCommandBufferSystem m_ecbWorld;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_ecbWorld = World.GetOrCreateSystem<EntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        PopulateFields();
        EntityCommandBuffer.ParallelWriter buffer = m_ecbWorld.CreateCommandBuffer().AsParallelWriter();
        var spawnJob = new SpawnEntitiesJob
        {
            ecb = buffer,
            entity = spawner.prefabEnemy,
        };

        spawnJob.Schedule(spawner.spawnAmount, 128).Complete();

        EntityManager.CreateEntity(typeof(EnemyPatternSystemEnabler));
    }

    private void PopulateFields()
    {
        Entities
            .WithoutBurst()
            .ForEach((ref EnemySpawnerComponent prefab) =>
            {
                spawner = prefab;
            }).Run();
    }

    [BurstCompatible]
    struct SpawnEntitiesJob : IJobParallelFor
    {
        public Entity entity;
        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute(int index)
        {
            ecb.Instantiate(index, entity);
        }
    }

    protected override void OnUpdate()
    {
        var buffer = m_ecbWorld.CreateCommandBuffer().AsParallelWriter();

        Dependency = Entities
            .WithAll<Disabled>()
            .ForEach((int entityInQueryIndex, in Entity e) =>
            {
                buffer.DestroyEntity(entityInQueryIndex, e);
            }).ScheduleParallel(Dependency);

        m_ecbWorld.AddJobHandleForProducer(Dependency);

    }
}
