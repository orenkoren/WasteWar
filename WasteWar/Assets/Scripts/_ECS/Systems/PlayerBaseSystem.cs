using Constants;
using Unity.Entities;
using Unity.Transforms;

public class PlayerBaseSystem : ComponentSystem
{
    protected override void OnStartRunning()
    {
        base.OnCreate();
        Entities.WithAny<PlayerBaseComponent>().ForEach((ref Translation translation) =>
        {
            GameConstants.Instance.PlayerBasePosition = translation;
        });
    }

    protected override void OnUpdate()
    {

    }
}
