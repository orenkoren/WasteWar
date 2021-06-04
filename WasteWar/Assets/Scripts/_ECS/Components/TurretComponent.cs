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
    public int RechargeTime;
    public int RotationTime;
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