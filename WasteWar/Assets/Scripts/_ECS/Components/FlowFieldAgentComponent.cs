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
    public int nextGridIndex;
    [HideInInspector]
    public int currentGridIndex;
    [HideInInspector]
    public int previousGridIndex;
}
