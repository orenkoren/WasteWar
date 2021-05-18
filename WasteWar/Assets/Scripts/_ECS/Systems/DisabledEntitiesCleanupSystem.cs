using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DisabledEntitiesCleanupSystem : SystemBase
{
    private EntityCommandBufferSystem m_ecbWorld;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_ecbWorld = World.GetOrCreateSystem<EntityCommandBufferSystem>();
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
