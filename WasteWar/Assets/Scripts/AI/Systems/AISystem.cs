using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class AISystem : ComponentSystem
{
    private Translation playerBasePosition;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        Entities.WithAny<PlayerBase>().ForEach((ref Translation translation) =>
        {
            playerBasePosition = translation;
        });
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        Translation playerBase = playerBasePosition;
        float deltaTime = Time.DeltaTime;
        Entities.WithAll<Attacker>().ForEach((ref Attacker attacker, ref Translation attackerPos) =>
        {
            float step = attacker.speed * deltaTime;
            attackerPos.Value = Vector3.MoveTowards(attackerPos.Value,
                                            new Vector3(playerBase.Value.x,
                                            attackerPos.Value.y, playerBase.Value.z), step);
        });
    }
}
