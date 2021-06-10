using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class HealthSystem : SystemBase
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
        if (!entitiesToTakeDamage.IsEmpty)
        {
            foreach (var entity in entitiesToTakeDamage)
            {
                if (EntityManager.HasComponent<HealthComponent>(entity))
                {
                    HealthComponent healthComp = EntityManager.GetComponentData<HealthComponent>(entity);
                    healthComp.CurrentHealth--;
                    EntityManager.SetComponentData(entity, healthComp);
                    if (healthComp.CurrentHealth <= 0)
                    {
                        HandleDeath(entity, healthComp.ShouldDestroyOnDeath);
                    }
                }
            }
            entitiesToTakeDamage.Clear();
        }

        Entities
            .WithAll<HealthComponent>()
            .WithoutBurst()
            .ForEach((Entity e, ref HealthComponent health) =>
            {
                if (EntityManager.HasComponent<HealthSync>(e))
                {
                    EntityManager.GetComponentObject<HealthSync>(e)
                        .SyncHealth((health.CurrentHealth / health.MaxHealth) * 100);
                }
            }).Run();
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
