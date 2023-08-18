using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WWP.Game;

public class PlayerModel : Unit
{
    public CharacterConfig Config { get; private set; }
    private HitConfig _hitConfig;

    public PlayerView View { get; private set; }

    public MyTransform Transform { get; private set; }

    public bool Active { get; private set; }

    public event Action<Unit, bool> Died;

    private float _timeSinceShoot;

    private float _distanceBetweenPoints;
    private SlimeShellModel _slime;

    public PlayerModel(CharacterConfig config)
    {
        Active = true;
        _distanceBetweenPoints = Vector3.Distance(SlimeShooterManager.Instance.shootPoint.position, SlimeShooterManager.Instance.runPoint.position);
        this.Transform = new MyTransform(SlimeShooterManager.Instance.shootPoint);

        Config = new(config);
        _timeSinceShoot = config.attackRate.value;

        _hitConfig = new HitConfig();
        _hitConfig.damage = config.damage.value;

        View = new PlayerView(this);
    }

    public void Update()
    {
        if (!Active) return;

        float deltaMovement = Config.speed.value * Time.deltaTime;

        if (CheckEnemy())
        {
            EnemyModel enemy = SlimeShooterManager.Instance.GetClosestEnemy();
            ExecuteShoot(enemy);
            float distance = Mathf.Abs(enemy.Transform.position.x - Transform.position.x);
            float scale = distance / _distanceBetweenPoints;
            scale = Mathf.Clamp01(scale);
            deltaMovement *= scale;
            Platform.Instance.MoveGround(-deltaMovement / 2);
        }
        else
        {
            Platform.Instance.MoveGround(-deltaMovement / 2);
            _timeSinceShoot = Config.attackRate.value;
        }

        View.Update();
    }

    public void MoveFromButton(bool up)
    {
        if (!Active) return;
        float deltaMovement = Config.speed.value * Time.deltaTime * (up ? 1 : -1);
        Vector3 position = Transform.position + new Vector3(0, deltaMovement, 0);
        position.y = Mathf.Clamp(position.y, 0.2f, 4.85f);
        Transform.position = position;
    }

    private void ExecuteShoot(EnemyModel enemy)
    {
        _timeSinceShoot += Time.deltaTime;

        if (_slime == null) _slime = new();

        if (_timeSinceShoot >= Config.attackRate.value)
        {
            if (enemy == null) return;
            _slime = new SlimeShellModel(enemy, Transform, _hitConfig);
            _timeSinceShoot = 0f;
        }
    }

    public void Enhance(Enhancement enhancement)
    {
        Parameter targetParameter = GetParameter(enhancement.targetParameterName);

        if (targetParameter != null)
        {
            RefreshParameter(targetParameter, enhancement.value);
        }
    }

    private Parameter GetParameter(string name)
    {
        Parameter targetParameter = null;

        switch (name)
        {
            case "AttackRate":
                targetParameter = Config.attackRate;
                break;
            case "Damage":
                targetParameter = Config.damage;
                break;
            case "Health":
                targetParameter = Config.health;
                break;
        }

        return targetParameter;
    }

    private void RefreshParameter(Parameter parameter, float value)
    {
        if (!parameter.isRate)
        {
            if (parameter.value + value <= parameter.limit) parameter.value += value;
            else parameter.value = parameter.limit;
        }

        else
        {
            if (parameter.limit <= parameter.value - value) parameter.value -= value;
            else parameter.value = parameter.limit;
        }

        View.RefreshHealth();
        _hitConfig.damage = Config.damage.value;
    }

    private bool CheckEnemy()
    {
        return SlimeShooterManager.Instance.CheckEnemy();
    }

    public void Recycle()
    {
        Active = false;
        _slime = null;
        View.Update();
        Died?.Invoke(this, true);
    }

    public void TakeHit(HitConfig hitConfig)
    {
        Config.health.value -= hitConfig.damage;
        View.RefreshHealth(hitConfig.damage);
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (Config.health.value <= 0)
            Recycle();
    }

    public bool CheckParameterIsMax(string parameterName)
    {
        Parameter parameter = GetParameter(parameterName);
        if (parameter != null)
        {
            return parameter.value == parameter.limit;
        }
        return false;
    }
}
