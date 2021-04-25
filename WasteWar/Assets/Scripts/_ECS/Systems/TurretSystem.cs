using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class TurretSystem : SystemBase
{
    private BuildPhysicsWorld m_physicsWorld;
    private EntityCommandBufferSystem m_ecbSystem;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
        m_ecbSystem = World.GetExistingSystem<EntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        CollisionWorld pworld = m_physicsWorld.PhysicsWorld.CollisionWorld;
        var ecb= m_ecbSystem.CreateCommandBuffer().AsParallelWriter();
        Dependency = Entities
          .WithBurst()
          .WithAll<TurretComponent>()
          .WithReadOnly(pworld)
          .ForEach((ref TurretComponent turret, in Translation translation, in Rotation rotation) =>
          {
              NativeList<RaycastHit> hits = new NativeList<RaycastHit>(Allocator.Temp);
              PerformRaycast(ref turret, translation, rotation, pworld, ref hits);
              foreach(var hit in hits)
              {
                  // add to a native list and consume in different job?
                  ecb.AddComponent<Disabled>(0, hit.Entity);
              }
          }).ScheduleParallel(Dependency);

        m_ecbSystem.AddJobHandleForProducer(Dependency);
    }

    private static void PerformRaycast(ref TurretComponent turret,
                        Translation translation, Rotation rotation, CollisionWorld pworld,
                        ref NativeList<RaycastHit> hits)
    {
        var rayInput = new RaycastInput
        {
            // TODO: turret destroys itself if start value is too close.. collisionfilter or change the start value
            Start = translation.Value + (math.forward(rotation.Value) * turret.startRange),
            End = translation.Value + (math.forward(rotation.Value) * turret.DetectionRadius),
            Filter = CollisionFilter.Default
        };
        for (float angle = -turret.coneSize; angle <= turret.coneSize; angle++)
        {
            pworld.CastRay(rayInput, ref hits);
            // TODO: math.distance()
            if (hits.Length > 0) return;

            float3 rayDir = math.mul(quaternion.AxisAngle(new float3(0, 1, 0), math.radians(angle)),
                                    math.forward(rotation.Value));
            rayInput.End = translation.Value + new float3 { x = 0, y = 2, z = 0 } +
                            (rayDir * turret.DetectionRadius);
        }
    }
}
