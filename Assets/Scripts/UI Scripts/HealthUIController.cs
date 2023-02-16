using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIController : MonoBehaviour
{
    [SerializeField] private GameObject healthFillObject;
    private Image _fill;
    private bool _isShaking;
    
    private void Start()
    {
        _isShaking = false;
        if (healthFillObject) _fill = healthFillObject.GetComponent<Image>();
    }

    // Randomly changes color of gauge fill for now.
    public void UpdateHealth(float value)
    {
        if (!_isShaking) StartCoroutine(Shake(0.1f, 10f));
        _fill.fillAmount = value;
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        _isShaking = true;
        var originalPosition = transform.position;
        var elapsed = 0.0f;
        while (elapsed < duration)
        {
            var x = Random.Range(-1f, 1f) * magnitude + originalPosition.x;
            var y = Random.Range(-1f, 1f) * magnitude + originalPosition.y;

            transform.position = new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return 0;
        }

        transform.position = originalPosition;
        _isShaking = false;
    }
}
