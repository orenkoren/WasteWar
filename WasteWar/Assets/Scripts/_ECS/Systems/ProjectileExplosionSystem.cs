using Unity.Entities;

public class ProjectileExplosionSystem : SystemBase
{
    private EntityCommandBufferSystem m_ecb;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_ecb = World.GetExistingSystem<EntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var buffer = m_ecb.CreateCommandBuffer().AsParallelWriter();
        Dependency = Entities
            .WithAll<MoveForwardComponent>()
            .ForEach((ref MoveForwardComponent moveForwardComp, in Entity e) =>
            {
                if (moveForwardComp.hasReached)
                    buffer.DestroyEntity(0, e);
            }).ScheduleParallel(Dependency);
    }
}
