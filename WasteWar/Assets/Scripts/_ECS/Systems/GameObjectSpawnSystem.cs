using Unity.Collections;
using Unity.Entities;

public class GameObjectSpawnSystem : SystemBase
{
    private TurretSystem turretSystem;
    private NativeList<Entity> entitiesToSpawnLaserBeams;

    protected override void OnCreate()
    {
        base.OnCreate();
        turretSystem = World.GetExistingSystem<TurretSystem>();
    }

    protected override void OnUpdate()
    {
        entitiesToSpawnLaserBeams = turretSystem.entitiesToSpawnBeams;
        foreach (var entity in entitiesToSpawnLaserBeams)
        {
            if (EntityManager.HasComponent<SpawnLaserBeams>(entity))
                EntityManager.GetComponentObject<SpawnLaserBeams>(entity).PlayBeamSound();

        }
        entitiesToSpawnLaserBeams.Clear();
    }
}
