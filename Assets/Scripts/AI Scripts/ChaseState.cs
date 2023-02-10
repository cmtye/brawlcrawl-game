using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Chase")]
public class ChaseState : AIState
{
    public override void Execute(AIController controller)
    {
        var distance = Vector3.Distance(controller.transform.position, GameManager.PlayerTransform.position);
        if (distance < controller.GetAttackRange())
        {
            controller.SetCurrentState(controller.attackState);
            controller.SetCountdown(controller.GetAttackDelay());
        }
        else if (distance > controller.GetVisionRadius() * 1.25f)
        {
            controller.SetCurrentState(controller.wanderState);
        }
        else
        {
            // Move towards target along the NavMesh
            controller.GetNavMeshAgent().destination = GameManager.PlayerTransform.position;
            controller.GetNavMeshAgent().isStopped = false;
        }
    }
}
