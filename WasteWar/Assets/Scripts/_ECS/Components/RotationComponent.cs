using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[GenerateAuthoringComponent]
public struct RotationComponent : IComponentData
{
    [HideInInspector]
    public float targetAngle;
    public float rotationTime;
}
