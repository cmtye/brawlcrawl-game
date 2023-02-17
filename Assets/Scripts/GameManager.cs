using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gaugeUI;
    [SerializeField] private GameObject healthUI;
    [SerializeField] private int currentCombo;
    [SerializeField] private int comboIncrement;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject pauseUI;
    private GaugeUIController _gaugeUI;
    private HealthUIController _healthUI;
    public static GameManager instance;
    private static GameObject _player;
    private List<int> _playerAbilityThresholds;
    [SerializeField] private Transform playerTransform;

    private bool _isPlayerTransformCached;
    private Scene _scene;

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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        _scene = SceneManager.GetActiveScene();
        if (instance == null) { instance = this; }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        currentCombo = 0;
        var playerScript = FindObjectOfType<PlayerController>();
        _player = playerScript.gameObject;
        _playerAbilityThresholds = playerScript.abilityThresholds;
        _isPlayerTransformCached = false;
        if (gaugeUI) _gaugeUI = gaugeUI.GetComponent<GaugeUIController>();
        if (healthUI) _healthUI = healthUI.GetComponent<HealthUIController>();
    }

    public void SetCameraTarget(Transform value, Vector3 newOffset, bool isPlayer)
    {
        mainCamera.GetComponent<CameraBehavior>().ChangeTarget(value, newOffset, isPlayer);
    }
    public int GetCombo() { return currentCombo; }

    public void UpdateHealthUI(int value)
    {
        switch (value)
        {
            case 5: 
                _healthUI.UpdateHealth(1f);
                break;
            case 4:
                _healthUI.UpdateHealth(0.8f);
                break;
            case 3:
                _healthUI.UpdateHealth(0.55f);
                break;
            case 2:
                _healthUI.UpdateHealth(0.35f);
                break;
            case 1:
                _healthUI.UpdateHealth(0.15f);
                break;
            case 0:
                _healthUI.UpdateHealth(0f);
                break;
        }
    }

    public void TogglePause(bool turnOn)
    {
        if (pauseUI)
        {
            if (turnOn)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                pauseUI.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                pauseUI.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
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

    public void ReloadScene()
    { 
        SceneManager.LoadScene(_scene.buildIndex);
    }
}
