using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gaugeUI;
    [SerializeField] private int currentCombo;
    [SerializeField] private int comboIncrement;
    private GaugeUIController _gaugeUI;
    public static GameManager instance;
    private static GameObject _player;
    private List<int> _playerAbilityThresholds;
    [SerializeField] private Transform playerTransform;

    private bool _isPlayerTransformCached;

    public static Transform PlayerTransform
    {
        get
        {
            if (instance._isPlayerTransformCached) return instance.playerTransform;
            
            instance.playerTransform = _player.transform;
            instance._isPlayerTransformCached = true;
            return instance.playerTransform;
        }
    }

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        
        currentCombo = 0;
        var playerScript = FindObjectOfType<PlayerController>();
        _player = playerScript.gameObject;
        _playerAbilityThresholds = playerScript.abilityThresholds;
        _isPlayerTransformCached = false;
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
    
    public void ResetCombo()
    {
        if (currentCombo >= _playerAbilityThresholds[2])
        {
            currentCombo = _playerAbilityThresholds[1];
        }
        else if (currentCombo >= _playerAbilityThresholds[1])
        {
            currentCombo = _playerAbilityThresholds[0];
        }
        else if (currentCombo >= _playerAbilityThresholds[0])
        {
            currentCombo = 0;
        }

        _gaugeUI.UpdateGauge(currentCombo);
    }
}
