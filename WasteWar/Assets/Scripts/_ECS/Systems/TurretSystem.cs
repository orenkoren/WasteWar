using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;

public class TurretSystem : SystemBase
{
    private BuildPhysicsWorld m_physicsWorld;
    private bool hasRun = false;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_physicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        CollisionWorld pworld = m_physicsWorld.PhysicsWorld.CollisionWorld;
        Entities
            .WithoutBurst()
            .WithAll<TurretComponent>()
            .ForEach((ref Translation translation, ref TurretComponent turret, ref Rotation rotation) =>
            {
                Dependency = new RayCastJob
                {
                    World = pworld,
                    SourceTranslation = translation,
                    SourceRotation = rotation,
                    TurrentComp = turret
                }.Schedule();
            }).Run();
    }

    struct RayCastJob : IJob
    {
        [ReadOnly] public CollisionWorld World;
        public Translation SourceTranslation;
        public RaycastHit ClosestHit;
        public Rotation SourceRotation;
        public TurretComponent TurrentComp;

        public void Execute()
        {
            if (TurrentComp.currentTarget != Entity.Null) return;
            var rayInput = new RaycastInput
            {
                Start = SourceTranslation.Value + new float3 { x = 0, y = 2, z = 0 },
                End = SourceTranslation.Value + new float3 { x = 0, y = 2, z = 0 } +
                                                (math.forward(SourceRotation.Value) * TurrentComp.DetectionRadius),
                Filter = CollisionFilter.Default
            };
            for (float angle = -45; angle <= 45; angle++)
            {
                World.CastRay(rayInput, out ClosestHit);
                TurrentComp.currentTarget = ClosestHit.Entity;

                UnityEngine.Debug.Log(TurrentComp.currentTarget.ToString());
                UnityEngine.Debug.DrawLine(rayInput.Start, rayInput.End);

                if (ClosestHit.Entity != Entity.Null) return;

                float3 rayDir = math.mul(quaternion.AxisAngle(new float3(0, 1, 0), math.radians(angle)),
                                        math.forward(SourceRotation.Value));
                rayInput.End = SourceTranslation.Value + new float3 { x = 0, y = 2, z = 0 } +
                                (rayDir * TurrentComp.DetectionRadius);
                
            }
        }
    }
}
