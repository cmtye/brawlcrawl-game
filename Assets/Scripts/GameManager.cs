using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject gaugeUI;
    [SerializeField] private int currentCombo;
    [SerializeField] private int comboIncrement;
    private GaugeUIController _gaugeUI;

    private void Awake()
    {
        instance = this;
        currentCombo = 0;
        if (gaugeUI) _gaugeUI = gaugeUI.GetComponent<GaugeUIController>();
    }

    public int GetCombo() { return currentCombo; }

    public void IncrementCombo()
    {
        currentCombo += comboIncrement;
        if (currentCombo > 100) currentCombo = 100;
        _gaugeUI.UpdateGauge(currentCombo);

    }
    
    public void SetCombo(int value)
    {
        currentCombo = value;
        if (currentCombo > 100) currentCombo = 100;
        _gaugeUI.UpdateGauge(currentCombo);
    }

    // TODO: Remove specified amount of tiers from combo gauge.
    public void ResetCombo()
    {
        currentCombo = 0;
        _gaugeUI.UpdateGauge(currentCombo);
    }
}
