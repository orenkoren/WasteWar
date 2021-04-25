using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TurretComponent : IComponentData
{
    public int DetectionRadius;
    public float startRange;
    public LayerMask HostileMask;
    public int HitsPerSecond;
    public int coneSize;
}
