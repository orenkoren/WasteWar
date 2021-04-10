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
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    EntityQuery m_TriggerGravityGroup;
    NativeList<Entity> entitiesToDestroy = new NativeList<Entity>(Allocator.Persistent);

    protected override void OnCreate()
    {
        base.OnCreate();
        m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
        m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        m_TriggerGravityGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
           {
                typeof(AICollisionComponent)
           }
        });
    }

    protected override void OnUpdate()
    {
        if (m_TriggerGravityGroup.CalculateEntityCount() == 0)
        {
            return;
        }

        Dependency = new CollisionJob
        {
            PhysicsVelocityGroup = GetComponentDataFromEntity<PhysicsVelocity>(),
            EntitiesList = entitiesToDestroy

        }.Schedule(m_StepPhysicsWorldSystem.Simulation,
            ref m_BuildPhysicsWorldSystem.PhysicsWorld, Dependency);

        var entities = entitiesToDestroy;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Dependency = Job
            .WithBurst()
            .WithCode(() =>
            {
                foreach (var entity in entities)
                {
                    ecb.DestroyEntity(entity);

                }
                entities.Clear();
            }).Schedule(Dependency);
        m_EndSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }

    [BurstCompile]
    private struct CollisionJob : ICollisionEventsJob
    {
        public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityGroup;

        public NativeList<Entity> EntitiesList;

        public void Execute(CollisionEvent collisionEvent)
        {
            // Check if one of the colliding entities is Attacker, if so, destroy it
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            bool isBodyADynamic = PhysicsVelocityGroup.HasComponent(entityA);
            bool isBodyBDynamic = PhysicsVelocityGroup.HasComponent(entityB);

            if (isBodyADynamic)
            {
                EntitiesList.Add(entityA);

            }
            else if (isBodyBDynamic)
            {
                EntitiesList.Add(entityB);
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        entitiesToDestroy.Dispose();
    }
}
