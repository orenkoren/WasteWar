using Constants;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class AISystem : SystemBase
{

    [BurstCompile]
    protected override void OnUpdate()
    {
        Translation playerBase = GameConstants.Instance.PlayerBasePosition;
        float deltaTime = Time.DeltaTime;
        Entities
            .WithAll<Attacker>()
            .ForEach(
                (ref Attacker attacker, ref Translation attackerPos) =>
                {
                    float step = attacker.speed * deltaTime;
                    attackerPos.Value = Vector3.MoveTowards(attackerPos.Value,
                                            new Vector3(playerBase.Value.x,
                                            attackerPos.Value.y, playerBase.Value.z), step);
                }
            )
            .ScheduleParallel();
    }
}
