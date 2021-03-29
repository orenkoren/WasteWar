using UnityEngine;
using UnityEngine.AI;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Attacker : IComponentData
{
    public Entity entity;
    [SerializeField]
    public LayerMask attackable;
    //[SerializeField]
    //public NavMeshAgent agent;
    [SerializeField]
    public float aggroRadius;
    
    public const float CAST_DISTANCE_PLACEHOLDER = 0.1f;

}
