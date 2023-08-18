using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView
{
    private readonly PlayerModel _model;

    private float _healthMax;
    private float _healthCurrent;

    private GameObject _playerObject;

    public PlayerView(PlayerModel model)
    {
        _model = model;

        _healthMax = model.Config.health.value;

        _healthCurrent = _healthMax;

        _playerObject = GameObject.Instantiate(SlimeShooterManager.Instance.playerPrefab, SlimeShooterManager.Instance.gameObjectContainer);

        _playerObject.transform.position = model.Transform.position;

        RefreshHealth();
    }

    public void Update()
    {
        if (_model.Active)
        {
            _playerObject.transform.position = _model.Transform.position;
        }
        else
        {
            if (_playerObject != null)
                GameObject.Destroy(_playerObject);
        }
    }
    public void RefreshHealth(float value)
    {
        _healthCurrent = _model.Config.health.value;
        var UIComponent = _playerObject.GetComponent<UIPrefabComponent>();
        UIComponent.healthbar.fillAmount = _healthCurrent / _healthMax;
        UIComponent.SpawnHealthText(value);
    }

    public void RefreshHealth()
    {
        _healthCurrent = _model.Config.health.value;
        var UIComponent = _playerObject.GetComponent<UIPrefabComponent>();
        UIComponent.healthbar.fillAmount = _healthCurrent / _healthMax;
    }
}
