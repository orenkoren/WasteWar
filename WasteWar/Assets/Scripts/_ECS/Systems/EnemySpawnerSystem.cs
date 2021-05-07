using Constants;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;

public class EnemySpawnerSystem : SystemBase
{
    private Random random;
    private EnemySpawnerComponent spawner;
    private EntityCommandBufferSystem m_ecbWorld;

    protected override void OnCreate()
    {
        base.OnCreate();
        random = new Random(56);
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
            random = random,
            playerBasePostion = GameConstants.Instance.PlayerBasePosition
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
        public Random random;
        public Translation playerBasePostion;

        public void Execute(int index)
        {
            var e = ecb.Instantiate(index, entity);
            var spawnLocation = new float3(random.NextFloat(0, 1000), 2, random.NextFloat(0, 1000));
            //ecb.SetComponent(index, e, new Translation
            //{
            //    Value = spawnLocation
            //});
            ecb.SetComponent(index, e, new Rotation
            {
                Value = quaternion.LookRotation(playerBasePostion.Value - spawnLocation, math.up())
            });
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
