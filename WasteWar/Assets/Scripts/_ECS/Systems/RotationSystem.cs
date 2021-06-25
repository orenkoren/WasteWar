using Unity.Entities;

public class RotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .ForEach((ref RotationComponent rotationComp, in Entity e) =>
             {
                 if (rotationComp.targetAngle != -999)
                 {
                     EntityManager.GetComponentObject<RotateHybridSync>(e)
                                  .RotateHybrid(rotationComp.targetAngle, rotationComp.rotationTime);
                 }
             }).Run();
    }
}
