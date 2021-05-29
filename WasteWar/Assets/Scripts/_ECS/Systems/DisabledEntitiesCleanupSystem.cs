using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DisabledEntitiesCleanupSystem : SystemBase
{
    private EntityCommandBufferSystem m_ecbWorld;
    private EnemyAmountTracker amountTracker;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_ecbWorld = World.GetOrCreateSystem<EntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        Entities
            .WithoutBurst()
            .ForEach((EnemyAmountTracker enemyAmountTracker) =>
        {
            amountTracker = enemyAmountTracker;
        }).Run();
    }
    protected override void OnUpdate()
    {
        var buffer = m_ecbWorld.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithoutBurst()
            .WithAll<Disabled>()
            .ForEach((in AttackerComponent attacker) =>
            {
                amountTracker.DecrementCurrentEnemyAmount();
            }).Run();
        Dependency = Entities
            .WithAll<Disabled>()
            .ForEach((int entityInQueryIndex, in Entity e) =>
            {
                buffer.DestroyEntity(entityInQueryIndex, e);
            }).ScheduleParallel(Dependency);

        m_ecbWorld.AddJobHandleForProducer(Dependency);
    }
}
