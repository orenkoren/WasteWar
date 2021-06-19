using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct FlowFieldAgentComponent : IComponentData
{
    [HideInInspector]
    public float3 currentDestination;
    [HideInInspector]
    public float3 finalDestination;
    [HideInInspector]
    public int currentGridIndex;
}
