using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class HealthSystem : ComponentSystem
{
    private AICollisionSystem collisionSystem;
    private NativeList<Entity> entitiesToTakeDamage;

    protected override void OnCreate()
    {
        base.OnCreate();
        collisionSystem = World.GetExistingSystem<AICollisionSystem>();
    }

    protected override void OnUpdate()
    {
        entitiesToTakeDamage = collisionSystem.entitiesToTakeDamage;
        if (entitiesToTakeDamage.IsEmpty)
            return;

        var healthComps = GetComponentDataFromEntity<HealthComponent>();

        foreach (var entity in entitiesToTakeDamage)
        {
            if (healthComps.HasComponent(entity))
            {
                healthComps[entity] = new HealthComponent
                {
                    Health = healthComps[entity].Health - 1,
                    ShouldDestroyOnDeath = healthComps[entity].ShouldDestroyOnDeath
                };
                if (healthComps[entity].Health <= 0)
                    HandleDeath(entity, healthComps[entity].ShouldDestroyOnDeath);
            }
        }
        entitiesToTakeDamage.Clear();
    }

    private void HandleDeath(Entity entity, bool shouldDestroy)
    {
        EntityManager.GetComponentObject<HybridEntitySync>(entity).DestroyHybrid();
        if (shouldDestroy)
        {
            EntityManager.DestroyEntity(entity);
        }
        else
            EntityManager.AddComponent<Disabled>(entity);
    }
}
