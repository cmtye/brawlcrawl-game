using UnityEngine;

namespace AI_Scripts
{
    [CreateAssetMenu(menuName = "AI/States/Chase")]
    public class ChaseState : AIState
    {
        public override void Execute(AIController controller)
        {
            controller.GetNavMeshAgent().stoppingDistance = controller.GetAttackRange();
            var distance = Vector3.Distance(controller.transform.position, GameManager.PlayerTransform.position);
            if (distance <= controller.GetAttackRange())
            {
                controller.GetAnimator().SetBool("isRunning", false);
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
                Debug.Log("here");
                controller.GetAnimator().SetBool("isRunning", true);
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
}
