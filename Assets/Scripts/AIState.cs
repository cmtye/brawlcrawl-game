using UnityEngine;

public abstract class AIState : ScriptableObject
{
    public abstract void Execute(AIController controller);
}