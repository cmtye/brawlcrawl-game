using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject gaugeUI;
    [SerializeField] private float currentCombo;
    [SerializeField] private float comboIncrement;
    private ComboGaugeController _gauge;

    private void Awake()
    {
        instance = this;
        currentCombo = 0f;
        if (gaugeUI) _gauge = gaugeUI.GetComponent<ComboGaugeController>();
    }

    public float GetCombo()
    {
        return currentCombo;
    }

    public void IncrementCombo()
    {
        currentCombo += comboIncrement;
        if (currentCombo > 1f) currentCombo = 1f;
        _gauge.UpdateGauge(currentCombo);

    }

    public void ResetCombo()
    {
        currentCombo = 0;
        _gauge.UpdateGauge(currentCombo);
    }
}
