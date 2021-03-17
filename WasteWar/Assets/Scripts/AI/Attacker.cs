using UnityEngine;
using UnityEngine.AI;

public class Attacker : MonoBehaviour
{
    [SerializeField]
    private LayerMask attackable;
    [SerializeField]
    private NavMeshAgent agent;
    [SerializeField]
    private float aggroRadius;
    
    private Vector3 agentPosition;

    private const float CAST_DISTANCE_PLACEHOLDER = 0.1f;

    void Start()
    {
        agentPosition = agent.transform.position;
    }

    void Update()
    {
        if (HasTarget())
        {
            agent.SetDestination(FindTarget());
        }
    }

    private Vector3 FindTarget()
    {
        var target = Physics.SphereCastAll(agentPosition, aggroRadius, Vector3.up, CAST_DISTANCE_PLACEHOLDER, attackable)[0];
        Debug.DrawLine(transform.position, target.transform.position);
        return target.transform.position;
    }

    private bool HasTarget()
    {
        return Physics.CheckSphere(agentPosition, aggroRadius, attackable);
    }
}
