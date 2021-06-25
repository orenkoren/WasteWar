using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveForwardSystem : SystemBase
{

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities
            .WithAll<MoveForwardComponent>()
            .ForEach(
                (ref Translation translation, ref Rotation rotation, ref MoveForwardComponent forwardComp) =>
                {
                    float step = forwardComp.speed * deltaTime;
                    translation.Value = MathUtilECS.MoveTowardsV2(translation.Value,
                                            forwardComp.destination, step);
                    rotation.Value = quaternion.LookRotation(
                          new float3(forwardComp.destination.x, 0, forwardComp.destination.z)
                                    - translation.Value, math.up());

                    if (math.distance(translation.Value, forwardComp.destination) < 5)
                        forwardComp.hasReached = true;
                }
            )
            .ScheduleParallel();
    }
}
