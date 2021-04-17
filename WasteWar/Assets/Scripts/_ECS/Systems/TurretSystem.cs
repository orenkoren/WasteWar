using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class TurretSystem : SystemBase
{
    private BuildPhysicsWorld m_physicsWorld;
    private NativeList<ColliderCastHit> m_hitsList;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
        m_hitsList = new NativeList<ColliderCastHit>(Allocator.Persistent);

    }

    protected override void OnUpdate()
    {
        CollisionWorld pworld = m_physicsWorld.PhysicsWorld.CollisionWorld;
        NativeList<ColliderCastHit> thisFrameHits = m_hitsList;
        Translation baseTranslation = new Translation { };
        TurretComponent baseTurret = new TurretComponent { };

        Entities
            .WithoutBurst()
            .WithAll<TurretComponent>()
            .ForEach((ref Translation translation, ref TurretComponent turret) =>
            {
                baseTranslation = translation;
                baseTurret = turret;
            }).Run();

        Dependency = new SphereCastJob
        {
            World = pworld,
            HitsList = thisFrameHits,
            SourceTranslation = baseTranslation,
            DetectionRadius = baseTurret.DetectionRadius
        }.Schedule();

        m_physicsWorld.AddInputDependency(Dependency);
        //Entities
        //    .WithoutBurst()
        //    .WithAll<TurretComponent>()
        //    .ForEach((Entity e, ref Translation translation, ref TurretComponent turret) =>
        //    {
        //        pworld.SphereCastAll(translation.Value, turret.DetectionRadius, new float3 { x = 0, y = 0.1f, z = 0 },
        //                            0.1f, ref thisFrameHits, CollisionFilter.Default);
        //    }).Run();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_hitsList.Dispose();
    }

    struct SphereCastJob : IJob
    {
        [ReadOnly] public CollisionWorld World;
        public NativeList<ColliderCastHit> HitsList;
        public Translation SourceTranslation;
        public int DetectionRadius;
        public Unity.Physics.RaycastHit closestHit;

        public void Execute()
        {
            //World.SphereCastAll(SourceTranslation.Value, DetectionRadius, new float3 { x = 0, y = 0.1f, z = 0 },
            //                    0.1f, ref HitsList, CollisionFilter.Default);
            //var collector = new AllHitsCollector<ColliderCastHit>();
            //World.SphereCastCustom(SourceTranslation.Value, DetectionRadius, new float3 { x = 0, y = 0.1f, z = 0 },
            //                       0.1f, ref collector, CollisionFilter.Default);
            for (int i = 0; i < 360; i++)
            {
                var rayInput = new RaycastInput
                {
                    Start = SourceTranslation.Value,
                    End = SourceTranslation.Value + (i * DetectionRadius),
                    Filter = CollisionFilter.Default
                };
                World.CastRay(rayInput, out closestHit);
            }
            
        }
    }
}
