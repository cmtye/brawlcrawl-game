using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "AI/States/Wander")]
public class WanderState : AIState
{
    public override void Execute(AIController controller)
    {
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
                    controller.GetWanderRadius(), controller.GetNavMeshMask())) return;
            controller.GetNavMeshAgent().destination = navMeshHit.position;
            controller.GetNavMeshAgent().isStopped = false;
        }
    }
}