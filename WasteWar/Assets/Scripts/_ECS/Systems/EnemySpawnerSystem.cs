using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemySpawnerSystem : SystemBase
{
    private Random random;
    private EntityCommandBufferSystem ecb;
    private EnemySpawnerComponent spawner;

    protected override void OnCreate()
    {
        base.OnCreate();
        random = new Random(56);
        Enabled = false;
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        ecb = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        Entities
            .WithoutBurst()
            .ForEach((ref EnemySpawnerComponent prefab) =>
            {
                spawner = prefab;
            }).Run();

        var buffer = ecb.CreateCommandBuffer().AsParallelWriter();
        var spawnJob = new SpawnEntitiesJob
        {
            ecb = buffer,
            entity = spawner.prefabEnemy,
            random = random
        };

        spawnJob.Schedule(spawner.spawnAmount, 128).Complete();
    }


    [BurstCompatible]
    struct SpawnEntitiesJob : IJobParallelFor
    {
        public Entity entity;
        public EntityCommandBuffer.ParallelWriter ecb;
        public Random random;

        public void Execute(int index)
        {
            var e = ecb.Instantiate(index, entity);
            ecb.SetComponent(index, e, new Translation
            {
                Value = new float3(random.NextFloat(0, 100), 2, random.NextFloat(0, 100))
            });
        }
    }

    protected override void OnUpdate()
    {

    }
}
