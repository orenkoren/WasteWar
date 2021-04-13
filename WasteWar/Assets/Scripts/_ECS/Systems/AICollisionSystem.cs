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
    NativeList<Entity> dynamicCollidedEntities = new NativeList<Entity>(Allocator.Persistent);

    protected override void OnCreate()
    {
        base.OnCreate();
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        Dependency = new CollisionJob
        {
            PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
            CurrentHealth = GetComponentDataFromEntity<PlayerBaseComponent>(),
            Disabled = GetComponentDataFromEntity<Disabled>(),
            Ecb = m_EndSimulationEcbSystem.CreateCommandBuffer()

        }.Schedule(m_StepPhysicsWorldSystem.Simulation,
            ref m_BuildPhysicsWorldSystem.PhysicsWorld, Dependency);

        var dynamicEntitiesCurrentFrame = dynamicCollidedEntities;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Dependency = Entities.
            ForEach((Entity e, ref PlayerBaseComponent playerBase) =>
            {
                if (playerBase.Health <= 0)
                    ecb.DestroyEntity(e);
            }).Schedule(Dependency);

        m_EndSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }

    [BurstCompile]
    private struct CollisionJob : ICollisionEventsJob
    {
        public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;

        public ComponentDataFromEntity<PlayerBaseComponent> CurrentHealth;

        public EntityCommandBuffer Ecb;

        public ComponentDataFromEntity<Disabled> Disabled;

        public void Execute(CollisionEvent collisionEvent)
        {
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            if (CurrentHealth.HasComponent(entityA))
            {
                CurrentHealth[entityA] = new PlayerBaseComponent { Health = CurrentHealth[entityA].Health - 1 };
            }
            else if (CurrentHealth.HasComponent(entityB))
            {
                CurrentHealth[entityB] = new PlayerBaseComponent { Health = CurrentHealth[entityB].Health - 1 };
            }

            bool isBodyADynamic = PhysicsVelocityGroup.HasComponent(entityA);
            bool isBodyBDynamic = PhysicsVelocityGroup.HasComponent(entityB);

            if (isBodyADynamic)
            {
                Ecb.AddComponent<Disabled>(entityA);
            }
            else if (isBodyBDynamic)
            {
                Ecb.AddComponent<Disabled>(entityB);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        dynamicCollidedEntities.Dispose();
    }
}
