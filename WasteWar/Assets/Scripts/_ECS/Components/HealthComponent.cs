using Unity.Entities;

[GenerateAuthoringComponent]
public struct HealthComponent : IComponentData
{
    public float MaxHealth;
    public float CurrentHealth;
    public bool ShouldDestroyOnDeath;
}