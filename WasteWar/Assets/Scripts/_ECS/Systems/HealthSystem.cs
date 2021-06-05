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

        foreach (var entity in entitiesToTakeDamage)
        {
            if (EntityManager.HasComponent<HealthComponent>(entity))
            {
                HealthComponent healthComp = EntityManager.GetComponentData<HealthComponent>(entity);
                healthComp.Health--;
                EntityManager.SetComponentData(entity, healthComp);
                if (healthComp.Health <= 0)
                {
                    HandleDeath(entity, healthComp.ShouldDestroyOnDeath);
                }
            }
        }
        entitiesToTakeDamage.Clear();
    }

    private void HandleDeath(Entity entity, bool shouldDestroy)
    {
        if (EntityManager.HasComponent<HybridDestructionSync>(entity))
            EntityManager.GetComponentObject<HybridDestructionSync>(entity).DestroyHybrid();
        if (shouldDestroy)
        {
            EntityManager.DestroyEntity(entity);
        }
        else
            EntityManager.AddComponent<Disabled>(entity);
    }
}
