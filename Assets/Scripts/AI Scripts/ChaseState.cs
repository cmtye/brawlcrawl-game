using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "AI/States/Chase")]
public class ChaseState : AIState
{
    public override void Execute(AIController controller)
    {
        controller.GetNavMeshAgent().stoppingDistance = 2f;
        var distance = Vector3.Distance(controller.transform.position, GameManager.PlayerTransform.position);
        if (distance < controller.GetAttackRange())
        {
            controller.SetCurrentState(controller.attackState);
            controller.SetCountdown(controller.GetAttackDelay());
        }
        else if (distance > controller.GetVisionRadius() * 1.5f)
        {
            controller.SetCurrentState(controller.wanderState);
        }
        else
        {
            // Move towards target along the NavMesh
            controller.GetNavMeshAgent().destination = GameManager.PlayerTransform.position;
            controller.GetNavMeshAgent().isStopped = false;
            var directionOfTravel = (controller.GetNavMeshAgent().destination - controller.transform.position)
                .normalized;
                
            switch (directionOfTravel.x)
            {
                case > 0 when !controller.GetCharacterMovement().facingRight:
                case < 0 when controller.GetCharacterMovement().facingRight:
                    controller.GetCharacterMovement().Flip();
                    break;
            }

        }
    }
}
