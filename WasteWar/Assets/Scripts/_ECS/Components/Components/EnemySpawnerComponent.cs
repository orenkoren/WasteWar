using Unity.Entities;

[GenerateAuthoringComponent]
public struct EnemySpawnerComponent : IComponentData
{
    public Entity prefabEnemy;
    public int spawnAmount;
}
