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

    protected override void OnStartRunning()
    {
        m_ecbWorld = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        m_gridSystem = World.GetExistingSystem<GridSystem>();
        EntityCommandBuffer.ParallelWriter buffer = m_ecbWorld.CreateCommandBuffer().AsParallelWriter();
        var destination = GameConstants.Instance.PlayerBasePosition;
        base.OnStartRunning();
        Entities
            .WithAll<FlowFieldAgentComponent>()
            .ForEach(
                (ref FlowFieldAgentComponent agent) =>
                {
                    agent.finalDestination = destination.Value;
                }
            )
            .ScheduleParallel();
    }

    protected override void OnUpdate()
    {
        //Func<float3, float3, float3> gridFunc = m_gridSystem.GetNextCellDestination;
        //Entities
        //    .WithAll<FlowFieldAgentComponent>()
        //    .ForEach(
        //        (ref FlowFieldAgentComponent agent, ref Translation translation) =>
        //        {
        //            agent.currentDestination = CalculateNextDestinationCell(agent, translation, gridFunc);
        //        }
        //    )
        //    .ScheduleParallel();
    }

    private static float3 CalculateNextDestinationCell(FlowFieldAgentComponent agent,
                    Translation translation, Func<float3, float3, float3> gridFunc)
    {
        Debug.Log(gridFunc(translation.Value, agent.finalDestination));

        return new float3(0, 0, 0);
    }
}
