using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class TurretSystem : SystemBase
{
    private BuildPhysicsWorld m_physicsWorld;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        CollisionWorld pworld = m_physicsWorld.PhysicsWorld.CollisionWorld;
        Entities
            .WithBurst()
            .WithAll<TurretComponent>()
            .WithReadOnly(pworld)
            .ForEach((ref TurretComponent turret, in Translation translation, in Rotation rotation) =>
            {
                PerformRaycast(ref turret, translation, rotation, pworld);
            }).ScheduleParallel();
    }

    private static void PerformRaycast(ref TurretComponent turret,
                        Translation translation, Rotation rotation, CollisionWorld pworld)
    {
        RaycastHit closestHit;
        if (turret.currentTarget != Entity.Null) return;
        var rayInput = new RaycastInput
        {
            Start = translation.Value + new float3 { x = 0, y = 2, z = 0 },
            End = translation.Value + new float3 { x = 0, y = 2, z = 0 } +
                                            (math.forward(rotation.Value) * turret.DetectionRadius),
            Filter = CollisionFilter.Default
        };
        for (float angle = -45; angle <= 45; angle++)
        {
            pworld.CastRay(rayInput, out closestHit);
            turret.currentTarget = closestHit.Entity;

            if (closestHit.Entity != Entity.Null) return;

            float3 rayDir = math.mul(quaternion.AxisAngle(new float3(0, 1, 0), math.radians(angle)),
                                    math.forward(rotation.Value));
            rayInput.End = translation.Value + new float3 { x = 0, y = 2, z = 0 } +
                            (rayDir * turret.DetectionRadius);
        }
    }
}
