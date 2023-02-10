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
        // Create capsule collider instead of sphere for better feeling Z-axis hit registration.
        var overlaps = Physics.OverlapSphere(controller.transform.position, 2, attackableLayers);
        foreach (Collider c in overlaps)
        {
            var healthBar = c.GetComponent<HealthBehavior>();
            if (healthBar) healthBar.TakeDamage(2);
        }
        controller.GetNavMeshAgent().isStopped = false;
        controller.SetCurrentState(controller.chaseState);
    }
}