using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct ParticleSpawnerComponent : IComponentData
{
    public Entity ParticleSystem;
    public Entity SpawnPosition;
}
