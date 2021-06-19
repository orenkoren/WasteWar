public class CounterConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Counter counter, EnemyAmountTracker enemyAmountTracker) =>
        {
            AddHybridComponent(counter);
            AddHybridComponent(enemyAmountTracker);
        });
    }
}
