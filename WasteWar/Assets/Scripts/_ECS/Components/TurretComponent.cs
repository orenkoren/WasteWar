using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TurretComponent : IComponentData
{
    public int DetectionRadius;
    public TurretBehavior behavior;
    public float startRange;
    public LayerMask HostileMask;
    public float RechargeTime;
    [Tooltip("The interval in which rotation command will be sent, thus beginning the rotation")]
    public float RotationCommandInterval;
    public int detectionConeSize;
    public int hitWidth;
    public Entity projectile;
    public float3 projectileSpawnLocation;
    public float projectileSpeed;
    [HideInInspector]
    public float rechargeTimer;
    [HideInInspector]
    public float rotationCooldown;
}

public enum TurretBehavior
{
    ClosestTarget,
    MostTargets
}