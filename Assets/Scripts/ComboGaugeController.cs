using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboGaugeController : MonoBehaviour
{
    [SerializeField] private GameObject radialFillObject;
    [SerializeField] private float additionPercentage;
    private Image _radialImage;

    private float _currentCombo;
    private bool _isShaking;
    // Start is called before the first frame update
    private void Start()
    {
        _isShaking = false;
        _currentCombo = 0f;
        if (radialFillObject)
        {
            _radialImage = radialFillObject.GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateGauge(bool wasHit)
    {
        if (wasHit)
        {
            ShakeGauge();
            _currentCombo = 0f;
        }
        else
        {
            ShakeGauge();
            _currentCombo += additionPercentage / 100f;
            if (_currentCombo > 1f)
            {
                _currentCombo = 1f;
            }

            _radialImage.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            _radialImage.fillAmount = _currentCombo;
        }
    }

    private void ShakeGauge()
    {
        if (!_isShaking)
        {
            StartCoroutine(Shake(0.1f, 10f));
        }
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
