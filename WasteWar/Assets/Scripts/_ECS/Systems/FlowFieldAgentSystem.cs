using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FlowFieldAgentSystem : SystemBase
{
    private EntityCommandBufferSystem m_ecbWorld;
    private GridSystem m_gridSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<EnemyPatternFinished>();
    }

    protected override void OnStartRunning()
    {
        m_ecbWorld = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        m_gridSystem = World.GetExistingSystem<GridSystem>();
        EntityCommandBuffer.ParallelWriter buffer = m_ecbWorld.CreateCommandBuffer().AsParallelWriter();
        var destination = GameConstants.Instance.PlayerBasePosition;
        base.OnStartRunning();
        //Entities
        //    .WithAll<FlowFieldAgentComponent>()
        //    .ForEach(
        //        (ref FlowFieldAgentComponent agent, in Translation translation, in Rotation rot) =>
        //        {
        //            agent.currentDestination = new float3(translation.Value.x, 200, translation.Value.z - 5000);
        //        }
        //    )
        //    .ScheduleParallel();
    }

    protected override void OnUpdate()
    {
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(56);
        var destination = GameConstants.Instance.PlayerBasePosition;
        int chanceToFlankInPercent = 0;
        int chanceToRookPercent = 0;
        int chanceToAvoidCollisions = 0;
        Entities
            .WithAll<FlowFieldAgentComponent>()
            .ForEach(
                (ref FlowFieldAgentComponent agent, in Translation translation) =>
                {
                    agent.currentDestination = new float3(translation.Value.x, 200, translation.Value.z - 5000);
                   
                    var rand = random.NextUInt(1, 100);
                    if (rand <= chanceToFlankInPercent)
                    {
                        agent.shouldFlank = true;
                        agent.shouldAvoidCollisions = true;
                    }
                    rand = random.NextUInt(1, 100);
                    if (rand <= chanceToRookPercent)
                    {
                        agent.IsRook = true;
                    }
                    rand = random.NextUInt(1, 100);
                    if (rand <= chanceToAvoidCollisions)
                    {
                        agent.shouldAvoidCollisions = true;
                    }

                    if (agent.shouldFlank)
                    {
                        agent.finalDestination = new float3(translation.Value.x, 3, random.NextFloat(350, 450));
                    }
                    else
                        agent.finalDestination = destination.Value;
                }
            )
            .ScheduleParallel();

        EntityManager.DestroyEntity(GetSingletonEntity<EnemyPatternFinished>());
    }

    private static float3 CalculateNextDestinationCell(FlowFieldAgentComponent agent,
                    Translation translation, Func<float3, float3, float3> gridFunc)
    {
        Debug.Log(gridFunc(translation.Value, agent.finalDestination));

        return new float3(0, 0, 0);
    }
}
