using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Attack")]
public class AttackState : AIState
{
    public override void Execute(AIController controller)
    {
        Debug.Log("Entered countdown");
        if (controller.GetCountdown() > 0)
        {
            controller.GetNavMeshAgent().isStopped = true;
            controller.SetCountdown(controller.GetCountdown() - Time.deltaTime);
            return;
        }
        Debug.Log("attacked lol");
        controller.GetNavMeshAgent().isStopped = false;

        controller.SetCurrentState(controller.chaseState);
    }
}