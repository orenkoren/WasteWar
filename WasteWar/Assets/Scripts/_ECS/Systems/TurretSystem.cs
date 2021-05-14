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
    public NativeList<Entity> entitiesToSpawnBeams = new NativeList<Entity>(Allocator.Persistent);

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
        m_ecbSystem = World.GetExistingSystem<EntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        CollisionWorld pworld = m_physicsWorld.PhysicsWorld.CollisionWorld;
        var ecb = m_ecbSystem.CreateCommandBuffer().AsParallelWriter();
        var entitiesToSpawnBeamsThisFrame = entitiesToSpawnBeams.AsParallelWriter();
        var deltaTime = Time.DeltaTime;
        Dependency = Entities
          .WithBurst()
          .WithAll<TurretComponent>()
          .WithReadOnly(pworld)
          .ForEach((ref TurretComponent turret, ref Entity e, in Translation translation, in Rotation rotation) =>
          {
              turret.rechargeTimer += deltaTime;
              if (turret.rechargeTimer >= turret.RechargeTime)
              {
                  turret.rechargeTimer = 0;
                  NativeList<RaycastHit> hits = new NativeList<RaycastHit>(Allocator.Temp);
                  PerformRaycast(ref turret, translation, rotation, pworld, ref hits);
                  if (hits.Length > 0)
                      entitiesToSpawnBeamsThisFrame.AddNoResize(e);
                  foreach (var hit in hits)
                  {
                      // add to a native list and consume in different job?
                      ecb.AddComponent<Disabled>(0, hit.Entity);
                  }
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

        var firstTargetAngle = ScanForTargets(turret, translation, rotation, pworld, rayInput);
        if (firstTargetAngle != -999)
        {
            for (float hitAngle = firstTargetAngle; hitAngle <= firstTargetAngle + turret.hitWidth / 2; hitAngle++)
            {
                NativeList<RaycastHit> oneAngleHits = GetSingleAngleHits(turret, translation, rotation, ref pworld, ref rayInput, hitAngle);
                UnityEngine.Debug.DrawLine(rayInput.Start, rayInput.End);
                hits.AddRange(oneAngleHits);
            }

            for (float hitAngle = firstTargetAngle; hitAngle >= firstTargetAngle - turret.hitWidth / 2; hitAngle--)
            {
                NativeList<RaycastHit> oneAngleHits = GetSingleAngleHits(turret, translation, rotation, ref pworld, ref rayInput, hitAngle);
                UnityEngine.Debug.DrawLine(rayInput.Start, rayInput.End);
                hits.AddRange(oneAngleHits);
            }
        }
    }

    private static NativeList<RaycastHit> GetSingleAngleHits(TurretComponent turret, Translation translation, Rotation rotation, ref CollisionWorld pworld, ref RaycastInput rayInput, float hitAngle)
    {
        NativeList<RaycastHit> oneAngleHits = new NativeList<RaycastHit>(Allocator.Temp);
        rayInput = ChangeRayDirection(turret, translation, rotation, rayInput, hitAngle);
        pworld.CastRay(rayInput, ref oneAngleHits);
        return oneAngleHits;
    }

    private static float ScanForTargets(TurretComponent turret, Translation translation, Rotation rotation,
     CollisionWorld pworld, RaycastInput rayInput)
    {

        for (float angle = 0; angle >= -turret.detectionConeSize; angle--)
        {
            rayInput = ChangeRayDirection(turret, translation, rotation, rayInput, angle);
            if (pworld.CastRay(rayInput)) return angle;
        }
        for (float angle = 0; angle <= turret.detectionConeSize; angle++)
        {
            rayInput = ChangeRayDirection(turret, translation, rotation, rayInput, angle);
            if (pworld.CastRay(rayInput)) return angle;
        }
        return -999;
    }

    private static RaycastInput ChangeRayDirection(TurretComponent turret, Translation translation, Rotation rotation, RaycastInput rayInput, float angle)
    {
        float3 rayDir = math.mul(quaternion.AxisAngle(new float3(0, 1, 0), math.radians(angle)),
                                            math.forward(rotation.Value));
        rayInput.End = translation.Value + new float3 { x = 0, y = 2, z = 0 } +
                        (rayDir * turret.DetectionRadius);
        return rayInput;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        entitiesToSpawnBeams.Dispose();
    }
}
