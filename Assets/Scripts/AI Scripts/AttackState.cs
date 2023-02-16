using System.Collections.Generic;
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
        controller.SetCountdown(controller.GetAttackDelay());
        controller.SetAttackStateCooldown(true);
        List<string> hitAlready = new List<string>();
        foreach (var c in overlaps)
        {
            if (!c.isTrigger) return;
            if (hitAlready.Contains(c.name)) return;
            
            var healthBar = c.GetComponent<HealthBehavior>();
            if (healthBar) { healthBar.TakeDamage(1); }
            hitAlready.Add(c.name);
        }
        controller.GetNavMeshAgent().isStopped = false;
        controller.SetCurrentState(controller.chaseState);
    }
}