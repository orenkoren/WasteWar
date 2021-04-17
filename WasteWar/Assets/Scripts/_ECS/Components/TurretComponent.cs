using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TurretComponent : IComponentData
{
    public int DetectionRadius;
    public LayerMask HostileMask;
    public int HitsPerSecond;
}
