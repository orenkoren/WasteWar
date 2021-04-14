using Unity.Entities;

[GenerateAuthoringComponent]
public struct HealthComponent : IComponentData
{
    public int Health;
    public bool ShouldDestroyOnDeath;
}