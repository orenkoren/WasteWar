using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class AICollisionSystem : SystemBase
{
    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;
    EndFixedStepSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    public NativeList<Entity> entitiesToTakeDamage = new NativeList<Entity>(Allocator.Persistent);

    protected override void OnCreate()
    {
        base.OnCreate();
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        Dependency = new CollisionJob
        {
            PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
            Ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter(),
            EntitiesToTakeDamage = entitiesToTakeDamage

        }.Schedule(m_StepPhysicsWorldSystem.Simulation,
            ref m_BuildPhysicsWorldSystem.PhysicsWorld, Dependency);

        m_EndSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }

    [BurstCompile]
    private struct CollisionJob : ICollisionEventsJob
    {
        public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;

        public NativeList<Entity> EntitiesToTakeDamage;

        public EntityCommandBuffer.ParallelWriter Ecb;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            EntitiesToTakeDamage.Add(entityA);
            EntitiesToTakeDamage.Add(entityB);

            bool isBodyADynamic = PhysicsVelocityGroup.HasComponent(entityA);
            bool isBodyBDynamic = PhysicsVelocityGroup.HasComponent(entityB);

            if (isBodyADynamic)
            {
                Ecb.AddComponent<Disabled>(entityA.Index, entityA);
            }
            else if (isBodyBDynamic)
            {
                Ecb.AddComponent<Disabled>(entityB.Index, entityB);
            }
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        entitiesToTakeDamage.Dispose();
    }
}
