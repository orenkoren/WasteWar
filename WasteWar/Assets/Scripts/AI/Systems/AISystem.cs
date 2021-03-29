using Reese.Nav;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class AISystem : ComponentSystem
{
    public struct AIGroup : IComponentData
    {
        public Attacker attacker;
        public NavAgentAuthoring agent;
    }

    protected override void OnUpdate()
    {
        //Entities.ForEach((ref AIGroup group) =>
        //{
        //    Vector3 agentPosition = group.agent.transform.position;
        //    if (HasTarget(group.agent, group))
        //    {
        //        //group.agent.(FindTarget(group.agent, group));
        //        Vector3 hit = FindTarget(group.agent, group);
        //        EntityManager.AddComponentData(GetSingletonEntity<NavAgent>(), new NavDestination
        //        {
        //            WorldPoint = hit,
        //            Teleport = false
        //        });
        //    }
        //});
    }

    private Vector3 FindTarget(NavAgentAuthoring ai, AIGroup group)
    {
        var target = Physics.SphereCastAll(ai.transform.position, group.attacker.aggroRadius, Vector3.up,
                                            Attacker.CAST_DISTANCE_PLACEHOLDER, group.attacker.attackable)[0];
        Debug.DrawLine(ai.transform.position, target.transform.position);
        return target.transform.position;
    }

    private bool HasTarget(NavAgentAuthoring ai, AIGroup group)
    {
        return Physics.CheckSphere(ai.transform.position, group.attacker.aggroRadius, group.attacker.attackable);
    }
}
