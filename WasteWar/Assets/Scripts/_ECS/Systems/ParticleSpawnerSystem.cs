using Unity.Entities;
using Unity.Transforms;

public class ParticleSpawnerSystem : ComponentSystem
{
    protected override void OnStartRunning()
    {
        Entities
            .WithAll<ParticleSpawnerComponent>()
            .ForEach((Entity e, ref ParticleSpawnerComponent particleSpawner, ref Translation translation) =>
            {
                var particleEntity = EntityManager.Instantiate(particleSpawner.ParticleSystem);
                LocalToWorld spawnPos = EntityManager.GetComponentData<LocalToWorld>(particleSpawner.SpawnPosition);
                EntityManager.SetComponentData(particleEntity, new Translation { Value = spawnPos.Position });
            });
    }

    protected override void OnUpdate()
    {

    }
}
