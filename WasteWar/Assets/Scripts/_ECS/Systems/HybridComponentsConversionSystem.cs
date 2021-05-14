public class HybridComponentsConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((SpawnLaserBeams syncComp) =>
        {
            AddHybridComponent(syncComp);
        });
    }
}
