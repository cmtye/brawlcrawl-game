using UnityEngine;

[CreateAssetMenu(menuName = "AI/States/Attack")]
public class AttackState : AIState
{
    [SerializeField] private LayerMask attackableLayers;
    public override void Execute(AIController controller)
    {
        controller.DisplayRange(controller.GetCountdown());
        if (controller.GetCountdown() > 0)
        {
            controller.GetNavMeshAgent().isStopped = true;
            controller.SetCountdown(controller.GetCountdown() - Time.deltaTime);
            return;
        }
        
        var overlaps = Physics.OverlapSphere(controller.transform.position, 2, attackableLayers);
        foreach (var c in overlaps)
        {
            if (c.isTrigger) continue;
            
            var healthBar = c.GetComponent<HealthBehavior>();
            if (healthBar) healthBar.TakeDamage(1);
        }
        controller.GetNavMeshAgent().isStopped = false;
        controller.SetCurrentState(controller.chaseState);
    }
}