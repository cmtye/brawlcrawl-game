using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour 
{
    private static readonly int Fill = Shader.PropertyToID("_Fill");
    private MaterialPropertyBlock _materialBlock;
    private MeshRenderer _meshRenderer;
    private Camera _mainCamera;
    private HealthBehavior _healthBehavior;

    private void Awake() 
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _materialBlock = new MaterialPropertyBlock();
        _healthBehavior = GetComponentInParent<HealthBehavior>();
    }

    // Hold reference to main scene camera that we adjust for.
    private void Start() { _mainCamera = Camera.main; }

    private void Update() 
    {
        // Only display health bar when on partial health. Allows border as a child for visibility.
        if (_healthBehavior.GetHealth() < _healthBehavior.GetMaxHealth()) {
            if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(true);
            if (_healthBehavior.GetHealth() <= 0) Destroy(gameObject, 0.6f);
            
            _meshRenderer.enabled = true;
            AlignCamera();
            UpdateParams();
        } 
        else 
        {
            if (transform.childCount > 0) transform.GetChild(0).gameObject.SetActive(false);
            _meshRenderer.enabled = false;
        }
    }

    private void UpdateParams() 
    {
        _meshRenderer.GetPropertyBlock(_materialBlock);
        _materialBlock.SetFloat(Fill, 1 - _healthBehavior.GetHealth() / _healthBehavior.GetMaxHealth());
        _meshRenderer.SetPropertyBlock(_materialBlock);
    }

    // Health bars rotate towards cameras location. Gives 3D view of health bar.
    private void AlignCamera()
    {
        if (!_mainCamera) return;
        
        var cameraTransform = _mainCamera.transform;
        var forward = transform.position - cameraTransform.position;
        forward.Normalize();
        
        var up = Vector3.Cross(forward, cameraTransform.right);
        transform.rotation = Quaternion.LookRotation(forward, up);
    }

}
