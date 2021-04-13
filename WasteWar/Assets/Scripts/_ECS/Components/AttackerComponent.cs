using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct AttackerComponent : IComponentData
{
    public float Damage;
    public float speed;
}
