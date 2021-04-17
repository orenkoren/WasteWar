using Constants;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class AISystem : SystemBase
{

    [BurstCompile]
    protected override void OnUpdate()
    {
        //Translation playerBase = GameConstants.Instance.PlayerBasePosition;
        //float deltaTime = Time.DeltaTime;
        //Entities
        //    .WithAll<AttackerComponent>()
        //    .ForEach(
        //        (ref AttackerComponent attacker, ref Translation attackerPos) =>
        //        {
        //            float step = attacker.speed * deltaTime;
        //            attackerPos.Value = MathUtilECS.MoveTowardsV2(attackerPos.Value,
        //                                    playerBase.Value, step);
        //        }
        //    )
        //    .ScheduleParallel();
    }
}
