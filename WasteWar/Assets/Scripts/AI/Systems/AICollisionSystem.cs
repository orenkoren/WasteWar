using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class AICollisionSystem : SystemBase
{
    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    StepPhysicsWorld m_StepPhysicsWorldSystem;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    EntityQuery m_TriggerGravityGroup;
    NativeList<Entity> dynamicCollidedEntities = new NativeList<Entity>(Allocator.Persistent);

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
            DynamicEntitiesList = dynamicCollidedEntities,
            CurrentHealth = GetComponentDataFromEntity<PlayerBase>(),
            Colliders = GetComponentDataFromEntity<PhysicsCollider>(),
            Positions = GetComponentDataFromEntity<Translation>()

        }.Schedule(m_StepPhysicsWorldSystem.Simulation,
            ref m_BuildPhysicsWorldSystem.PhysicsWorld, Dependency);

        var dynamicEntitiesCurrentFrame = dynamicCollidedEntities;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Dependency = Job
            .WithBurst()
            .WithCode(() =>
            {
                foreach (var entity in dynamicEntitiesCurrentFrame)
                {
                    ecb.DestroyEntity(entity);
                }
                dynamicEntitiesCurrentFrame.Clear();
            }).Schedule(Dependency);

        Dependency = Entities.
            ForEach((Entity e, ref PlayerBase playerBase) =>
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

        public NativeList<Entity> DynamicEntitiesList;

        public ComponentDataFromEntity<PlayerBase> CurrentHealth;

        public ComponentDataFromEntity<PhysicsCollider> Colliders;

        public ComponentDataFromEntity<Translation> Positions;

        public void Execute(CollisionEvent collisionEvent)
        {
            // Check if one of the colliding entities is Attacker, if so, destroy it
            Entity entityA = collisionEvent.EntityA;
            Entity entityB = collisionEvent.EntityB;

            if (CurrentHealth.HasComponent(entityA))
            {
                CurrentHealth[entityA] = new PlayerBase { Health = CurrentHealth[entityA].Health - 1 };
            }
            else if (CurrentHealth.HasComponent(entityB))
            {
                CurrentHealth[entityB] = new PlayerBase { Health = CurrentHealth[entityB].Health - 1 };
            }

            bool isBodyADynamic = PhysicsVelocityGroup.HasComponent(entityA);
            bool isBodyBDynamic = PhysicsVelocityGroup.HasComponent(entityB);

            if (isBodyADynamic)
            {
                DynamicEntitiesList.Add(entityA);
                Positions[entityA] = new Translation { Value = new Unity.Mathematics.float3 { x = 0, y = -100, z = 0} };

            }
            else if (isBodyBDynamic)
            {
                DynamicEntitiesList.Add(entityB);
                Positions[entityB] = new Translation { Value = new Unity.Mathematics.float3 { x = 0, y = -100, z = 0} };
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        dynamicCollidedEntities.Dispose();
    }
}
