using Unity.Entities;
using Unity.Rendering;

public class DisableRenderingSystem : SystemBase
{
    private EntityCommandBufferSystem m_ecbSystem;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_ecbSystem = World.GetExistingSystem<EntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ecbSystem.CreateCommandBuffer().AsParallelWriter();
        Dependency = Entities
          .WithBurst()
          .WithAll<DisableRenderingComponent>()
          .ForEach((ref Entity e) =>
          {
              ecb.AddComponent<DisableRendering>(0, e);
              // ecb.RemoveComponent<RenderMesh>(0, e);
          }).ScheduleParallel(Dependency);

        m_ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
