using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[GenerateAuthoringComponent]
public struct RotationComponent : IComponentData
{
    [HideInInspector]
    public float targetAngle;
    [HideInInspector]
    public float3 targetLocation;
    public float rotationTime;
}
