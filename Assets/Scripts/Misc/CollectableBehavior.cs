using System;
using UnityEngine;

namespace Misc
{
    public class CollectableBehavior : MonoBehaviour
    {
        [SerializeField] private ParticleSystem grabbedFX;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            GameManager.instance.ObtainCollectable();
            
            DeactivateRenderer();
            EmitGrabbedFX();
            Destroy(gameObject, 1f);
        }
        
        private void DeactivateRenderer()
        {
            foreach (var r in GetComponents<Renderer>())
            {
                r.enabled = false;
            }
        }
        private void EmitGrabbedFX() { if (grabbedFX) grabbedFX.Play(); }
    }
}
