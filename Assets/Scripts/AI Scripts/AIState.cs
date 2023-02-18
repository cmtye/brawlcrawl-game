using UnityEngine;

namespace AI_Scripts
{
    public abstract class AIState : ScriptableObject
    {
        public abstract void Execute(AIController controller);
    }
}