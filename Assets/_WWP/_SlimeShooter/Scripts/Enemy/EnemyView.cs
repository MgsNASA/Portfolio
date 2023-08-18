using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView
{
    private readonly EnemyModel _model;

    private float _healthMax;
    private float _healthCurrent;

    private GameObject _enemyObject;

    public EnemyView(EnemyModel model)
    {
        _model = model;

        _healthMax = model.Config.health.value;

        _healthCurrent = _healthMax;

        _enemyObject = GameObject.Instantiate(SlimeShooterManager.Instance.enemyPrefab, SlimeShooterManager.Instance.gameObjectContainer);

        _enemyObject.transform.position = model.Transform.position;

        RefreshHealth();
    }

    public void Update()
    {
        if (_model.Active)
        {
            _enemyObject.transform.position = _model.Transform.position;
        }
        else
        {
            if (_enemyObject != null)
                GameObject.Destroy(_enemyObject);
        }
    }

    public void RefreshHealth(float value)
    {
        _healthCurrent = _model.Config.health.value;
        var UIComponent = _enemyObject.GetComponent<UIPrefabComponent>();
        UIComponent.healthbar.fillAmount = _healthCurrent / _healthMax;
        UIComponent.SpawnHealthText(value);
    }

    public void RefreshHealth()
    {
        _healthCurrent = _model.Config.health.value;
        var UIComponent = _enemyObject.GetComponent<UIPrefabComponent>();
        UIComponent.healthbar.fillAmount = _healthCurrent / _healthMax;
    }
}
