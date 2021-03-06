using Constants;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class AISystem : SystemBase
{

    [BurstCompile]
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities
            .WithAll<AttackerComponent>()
            .ForEach(
                (ref AttackerComponent attacker, ref Translation attackerPos, ref Rotation rotation,
                        ref FlowFieldAgentComponent agent) =>
                {
                    float step = attacker.speed * deltaTime;
                    attackerPos.Value = MathUtilECS.MoveTowardsV2(attackerPos.Value,
                                            new float3(agent.currentDestination.x , 3, agent.currentDestination.z), step);
                    rotation.Value = quaternion.LookRotation(
                          new float3(agent.currentDestination.x, 3, agent.currentDestination.z)
                                    - attackerPos.Value, math.up());
                }
            )
            .ScheduleParallel();
    }
}
