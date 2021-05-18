using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MoveForwardComponent : IComponentData
{
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public float3 destination;
}
