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
    public NativeList<Entity> entitiesToSpawnBeams = new NativeList<Entity>(1000, Allocator.Persistent);

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
        m_ecbSystem = World.GetExistingSystem<EntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        CollisionWorld pworld = m_physicsWorld.PhysicsWorld.CollisionWorld;
        EntityCommandBuffer.ParallelWriter ecb = m_ecbSystem.CreateCommandBuffer().AsParallelWriter();
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
                  PerformRaycast(ref turret, translation, rotation, pworld, ref hits, ecb);
                  if (hits.Length > 0)
                  {
                      //var projectile = ecb.Instantiate(0, turret.projectile);
                      //ecb.SetComponent(0, projectile, translation);
                      //ecb.SetComponent(0, projectile,
                      //    new MoveForwardComponent { speed = turret.projectileSpeed, destination = math.normalize(hits[0].Position) * 10000 });
                      entitiesToSpawnBeamsThisFrame.AddNoResize(e);
                  }
                  foreach (var hit in hits)
                  {
                      //var projectile = ecb.Instantiate(0, turret.projectile);
                      //var projectileDestination = (hit.Position - translation.Value) * 5000;
                      //UnityEngine.Debug.DrawLine(translation.Value, projectileDestination);
                      //ecb.SetComponent(0, projectile, translation);
                      //ecb.SetComponent(0, projectile,
                      //    new MoveForwardComponent
                      //    {
                      //        speed = turret.projectileSpeed,
                      //        destination = projectileDestination
                      //    });
                      // add to a native list and consume in different job?
                      //ecb.AddComponent<Disabled>(0, hit.Entity);
                  }
              }
          }).ScheduleParallel(Dependency);

        m_ecbSystem.AddJobHandleForProducer(Dependency);
    }

    private static void PerformRaycast(ref TurretComponent turret,
                        Translation translation, Rotation rotation, CollisionWorld pworld,
                        ref NativeList<RaycastHit> hits, EntityCommandBuffer.ParallelWriter ecb)
    {
        var collisionFilter = new CollisionFilter()
        {
            BelongsTo = ~0u,
            CollidesWith = 1 << 24,
            GroupIndex = 0
        };

        var rayInput = new RaycastInput
        {
            // TODO: turret destroys itself if start value is too close.. collisionfilter or change the start value
            Start = translation.Value + (math.forward(rotation.Value) * turret.startRange),
            End = translation.Value + (math.forward(rotation.Value) * turret.DetectionRadius),
            Filter = collisionFilter
        };

        var firstTargetAngle = ScanForTargets(turret, translation, rotation, pworld, rayInput);
        if (firstTargetAngle != -999)
        {
            for (float hitAngle = firstTargetAngle; hitAngle < firstTargetAngle + turret.hitWidth / 2; hitAngle++)
            {
                AddSingleAngleHits(turret, translation, rotation, ref pworld, ref hits, ref rayInput, hitAngle);
                SpawnProjectile(turret, translation, ref ecb, ref rayInput);
            }

            for (float hitAngle = firstTargetAngle; hitAngle >= firstTargetAngle - turret.hitWidth / 2; hitAngle--)
            {
                AddSingleAngleHits(turret, translation, rotation, ref pworld, ref hits, ref rayInput, hitAngle);
                SpawnProjectile(turret, translation, ref ecb, ref rayInput);
            }
        }
    }

    private static void SpawnProjectile(TurretComponent turret, Translation translation, ref EntityCommandBuffer.ParallelWriter ecb, ref RaycastInput rayInput)
    {
        var projectile = ecb.Instantiate(0, turret.projectile);
        var projectileDestination = (rayInput.End - rayInput.Start) * 5000;
        Translation newTrans = new Translation { Value = translation.Value + turret.projectileSpawnLocation };
        UnityEngine.Debug.DrawLine(newTrans.Value, projectileDestination);
        ecb.SetComponent(0, projectile, newTrans);
        ecb.SetComponent(0, projectile,
            new MoveForwardComponent
            {
                speed = turret.projectileSpeed,
                destination = projectileDestination
            });
    }

    private static void AddSingleAngleHits(TurretComponent turret, Translation translation, Rotation rotation, ref CollisionWorld pworld, ref NativeList<RaycastHit> hits, ref RaycastInput rayInput, float hitAngle)
    {
        NativeList<RaycastHit> oneAngleHits = GetSingleAngleHits(turret, translation, rotation, ref pworld, ref rayInput, hitAngle);
        UnityEngine.Debug.DrawLine(rayInput.Start, rayInput.End);
        hits.AddRange(oneAngleHits);
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
        int mostTargetsHit = 0;
        float mostTargetsAngle = -999;
        NativeList<RaycastHit> oneAngleHits = new NativeList<RaycastHit>(Allocator.Temp);

        for (float angle = 0; angle >= -turret.detectionConeSize; angle--)
        {
            rayInput = ChangeRayDirection(turret, translation, rotation, rayInput, angle);
            pworld.CastRay(rayInput, ref oneAngleHits);
            if (oneAngleHits.Length > mostTargetsHit)
            {
                mostTargetsHit = oneAngleHits.Length;
                mostTargetsAngle = angle;
            }
            //if (pworld.CastRay(rayInput)) return angle;
        }
        for (float angle = 0; angle <= turret.detectionConeSize; angle++)
        {
            rayInput = ChangeRayDirection(turret, translation, rotation, rayInput, angle);
            pworld.CastRay(rayInput, ref oneAngleHits);
            if (oneAngleHits.Length > mostTargetsHit)
            {
                mostTargetsHit = oneAngleHits.Length;
                mostTargetsAngle = angle;
            }
            //if (pworld.CastRay(rayInput)) return angle;
        }
        return mostTargetsAngle;
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
