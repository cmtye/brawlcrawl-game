using UnityEngine;
using UnityEngine.AI;

namespace AI_Scripts
{
    [CreateAssetMenu(menuName = "AI/States/Wander")]
    public class WanderState : AIState
    {
        public override void Execute(AIController controller)
        {
            controller.GetNavMeshAgent().isStopped = false;
            controller.GetAnimator().SetBool("isRunning", false);
            controller.GetNavMeshAgent().stoppingDistance = 0.1f;
            var distance = Vector3.Distance(controller.transform.position, GameManager.PlayerTransform.position);
            if (distance < controller.GetVisionRadius())
            {
                controller.SetCurrentState(controller.chaseState);
            }
            else
            {
                // Move randomly within the NavMesh
                var randomPos = controller.transform.position + Random.insideUnitSphere * controller.GetWanderRadius();
                if (!NavMesh.SamplePosition(randomPos, out var navMeshHit, 
                        controller.GetWanderRadius(), controller.GetNavMeshMask())) 
                    return;
            
                if (controller.GetRemainingTime() < controller.GetTimeBetweenWaypoints() + Random.Range(-0.5f,0.5f))
                {
                    controller.SetRemainingTime(controller.GetRemainingTime() + Time.deltaTime);
                }
                else
                {
                    controller.GetAnimator().SetBool("isRunning", true);
                    controller.GetNavMeshAgent().destination = navMeshHit.position;
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
                    controller.SetRemainingTime(0);
                }
            }
        }
    }
}