using System.Collections.Generic;
using Character_Scripts;
using UnityEngine;

namespace AI_Scripts
{
    [CreateAssetMenu(menuName = "AI/States/Attack")]
    public class AttackState : AIState
    {
        [SerializeField] private LayerMask attackableLayers;
        public override void Execute(AIController controller)
        {
            controller.GetProgress().SetActive(true);
            controller.GetOutline().SetActive(true);
        
            if (controller.GetCountdown() > 0)
            {

                if (controller.GetNavMeshAgent().enabled)
                {
                    controller.GetAnimator().SetBool("chargingAttack", true);
                    controller.GetProgress().transform.localScale = Vector3.SmoothDamp(controller.GetProgress().transform.localScale,
                        new Vector3(controller.GetOutline().transform.localScale.x - 4, controller.GetOutline().transform.localScale.y - 4, controller.GetOutline().transform.localScale.z - 4), ref controller.GetProgressVector(), 0.7f);
                    controller.GetNavMeshAgent().isStopped = true;
                    controller.SetCountdown(controller.GetCountdown() - Time.deltaTime);
                    return;
                }

                controller.GetAnimator().SetBool("chargingAttack", false);
                controller.GetProgress().transform.localScale = new Vector3(1,1,1);
                controller.GetProgress().SetActive(false);
                controller.GetOutline().SetActive(false);
                controller.SetCountdown(controller.GetAttackDelay());
                controller.SetCurrentState(controller.chaseState);
                return;
            }
            
            controller.GetProgress().transform.localScale = new Vector3(1,1,1);
            controller.GetProgress().SetActive(false);
            controller.GetOutline().SetActive(false);
            var overlaps = Physics.OverlapSphere(controller.transform.position, 2.2f, attackableLayers);
            controller.GetAnimator().SetTrigger("isAttacking");
            controller.GetAnimator().SetBool("chargingAttack", false);
            controller.SetAttackStateCooldown(true);
            controller.SetCountdown(controller.GetAttackDelay());
            List<string> hitAlready = new List<string>();
            foreach (var c in overlaps)
            {
                if (!c.isTrigger) return;
                if (hitAlready.Contains(c.name)) return;
            
                var healthBar = c.GetComponent<HealthBehavior>();
                if (healthBar) { healthBar.TakeDamage(1, controller.transform.position); }
                hitAlready.Add(c.name);
            }
            controller.GetNavMeshAgent().isStopped = false;
            controller.SetCurrentState(controller.chaseState);
        }
    }
}