using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct EnemySpawnerComponent : IComponentData
{
    public Entity prefabEnemy;
    public int spawnAmount;
    public SpawnPattern pattern;
}

public enum SpawnPattern
{
    Asterix,
    Circle,
    Zattack,
    Square,
    Focused
}