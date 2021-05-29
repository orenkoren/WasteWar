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
        RequireSingletonForUpdate<EnemySpawnerSystemEnabler>();
    }
    private void PopulateFields()
    {
        Entities
            .WithoutBurst()
            .ForEach((EnemyAmountTracker amountTracker, ref EnemySpawnerComponent prefab ) =>
            {
                spawner = prefab;
                amountTracker.SetCurrentEnemyAmount(prefab.spawnAmount);
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
        m_ecbWorld = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        PopulateFields();
        EntityCommandBuffer.ParallelWriter buffer = m_ecbWorld.CreateCommandBuffer().AsParallelWriter();
        var spawnJob = new SpawnEntitiesJob
        {
            ecb = buffer,
            entity = spawner.prefabEnemy,
        };

        spawnJob.Schedule(spawner.spawnAmount, 128).Complete();

        EntityManager.CreateEntity(typeof(EnemyPatternSystemEnabler));
        EntityManager.DestroyEntity(GetSingletonEntity<EnemySpawnerSystemEnabler>());
    }
}
