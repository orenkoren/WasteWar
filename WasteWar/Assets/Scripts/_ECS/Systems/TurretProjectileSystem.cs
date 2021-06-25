using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class TurretProjectileSystem : SystemBase
{
    public NativeList<Entity> entitiesToSpawnBeams = new NativeList<Entity>(1000, Allocator.Persistent);
    private EntityCommandBufferSystem m_ecb;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_ecb = World.GetExistingSystem<EntityCommandBufferSystem>();
    }


    protected override void OnUpdate()
    {
        var entitiesToSpawnBeamsThisFrame = entitiesToSpawnBeams.AsParallelWriter();
        var buffer = m_ecb.CreateCommandBuffer().AsParallelWriter();
        var deltaTime = Time.DeltaTime;
        Dependency = Entities
            .WithAll<TurretComponent>()
            .ForEach((ref TurretComponent turret, in Entity e, in RotationComponent rot, in Translation translation) =>
            {
                turret.rechargeTimer += deltaTime;
                if (turret.rechargeTimer < turret.RechargeTime || !turret.hasTarget)
                    return;

                turret.rechargeTimer = 0;
                SpawnProjectile(turret, ref buffer, rot.targetLocation, translation);
                entitiesToSpawnBeamsThisFrame.AddNoResize(e);

                void SpawnProjectile(TurretComponent turret, ref EntityCommandBuffer.ParallelWriter ecb,
                                    float3 targetLocation, Translation translation)
                {
                    var projectile = ecb.Instantiate(0, turret.projectile);
                    var projectileDestination = turret.currentTargetLocation;
                    float3 newOffset = math.mul(quaternion.LookRotation(targetLocation -translation.Value,
                                                new float3(0, 1, 0)),
                                                turret.projectileSpawnLocation);
                    UnityEngine.Debug.DrawLine(translation.Value, newOffset);
                    Translation newTrans = new Translation { Value = translation.Value + newOffset };
                    ecb.SetComponent(0, projectile, newTrans);
                    ecb.SetComponent(0, projectile,
                        new MoveForwardComponent
                        {
                            speed = turret.projectileSpeed,
                            destination = projectileDestination
                        });
                }
            }).ScheduleParallel(Dependency);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        entitiesToSpawnBeams.Dispose();
    }
}
