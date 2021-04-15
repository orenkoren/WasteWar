using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public class EnemySpawnerSystem : SystemBase
{
    private Random random;
    private EnemySpawnerComponent spawner;
    private RenderMesh m_renderer;

    protected override void OnCreate()
    {
        base.OnCreate();
        random = new Random(56);
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        PopulateFields();
        var buffer = new EntityCommandBuffer(Allocator.TempJob);
        var desc = new RenderMeshDescription(m_renderer.mesh, m_renderer.material,
                                            UnityEngine.Rendering.ShadowCastingMode.Off, receiveShadows: false);
        var prototype = EntityManager.CreateEntity();
        RenderMeshUtility.AddComponents(
            prototype,
            EntityManager,
            desc);
        EntityManager.AddComponentData(prototype, new LocalToWorld());
        EntityManager.AddComponentData(prototype, new Translation());
        EntityManager.AddComponentData(prototype, new AttackerComponent { speed = 7 });

        var parallelBuffer = buffer.AsParallelWriter();
        var spawnJob = new SpawnEntitiesJob
        {
            ecb = parallelBuffer,
            entity = prototype,
            random = random
        };

        spawnJob.Schedule(spawner.spawnAmount, 128).Complete();
        buffer.Playback(EntityManager);
        buffer.Dispose();
        EntityManager.DestroyEntity(prototype);
    }

    private void PopulateFields()
    {
        Entities
            .WithoutBurst()
            .ForEach((ref EnemySpawnerComponent prefab) =>
            {
                spawner = prefab;
            }).Run();
        m_renderer = EntityManager.GetSharedComponentData<RenderMesh>(spawner.prefabEnemy);
        //foreach (var item in EntityManager.GetComponentTypes(spawner.prefabEnemy))
        //{
        //    if(item.GetType() == typeof(IComponentData))
        //    {
        //        IComponentData typeData = (IComponentData)item.GetType();
        //        EntityManager.GetComponentData(spawner.prefabEnemy);
        //    }
        //}
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
