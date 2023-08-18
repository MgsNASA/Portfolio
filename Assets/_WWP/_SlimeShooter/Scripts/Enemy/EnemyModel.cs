using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WWP.Game;

public class EnemyModel : Unit
{
    public EnemyView View { get; private set; }

    public EnemyConfig Config { get; private set; }
    private HitConfig _hitConfig;

    public MyTransform Transform { get; private set; }
    private float _offset;

    public bool Active { get; private set; }

    public event Action<Unit, bool> Died;

    private float _timeSinceAttack;

    public EnemyModel(EnemyConfig config, Transform spawnTransform, float offset)
    {
        Active = true;
        this.Transform = new MyTransform(spawnTransform);
        Config = new(config);

        _offset = offset;
        this.Transform.position.y += _offset; 

        _hitConfig = new HitConfig();
        _hitConfig.damage = config.damage.value;

        View = new(this);
    }

    public void Update()
    {
        if (!Active) return;

        if (MoveToPlayer())
        {
            ExecuteAttack();
        }

        View.Update();
    }

    private bool MoveToPlayer()
    {
        Vector3 targetPosition = SlimeShooterManager.Instance.Player.Transform.position;
        targetPosition.x += Config.attackRange.value;
        targetPosition.z += _offset;

        float deltaMovement = Config.speed.value * Time.deltaTime;
        this.Transform.position = Vector3.MoveTowards(this.Transform.position, targetPosition, deltaMovement);

        if ((this.Transform.position - targetPosition).magnitude < Config.attackRange.value)
        {
            return true;
        }


        return false;
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
            Recycle(true);
    }

    private void ExecuteAttack()
    {
        _timeSinceAttack += Time.deltaTime;

        if (_timeSinceAttack >= Config.attackRate.value)
        {
            new SlimeShellModel(SlimeShooterManager.Instance.Player, Transform, _hitConfig);
            //SlimeShooterManager.Instance.Player.TakeHit(_hitConfig);
            _timeSinceAttack = 0;
        }
    }

    public void Recycle(bool inProcess)
    {
        Active = false;
        View.Update();
        Died?.Invoke(this, inProcess);
    }
}

[Serializable]
public class EnemyConfig : CharacterConfig
{
    public int killValue;

    public EnemyConfig (EnemyConfig config)
    {
        speed = new();
        attackRate = new();
        attackRange = new();
        health = new();
        damage = new();

        speed.value = config.speed.value;
        speed.limit = config.speed.limit;
        speed.isRate = false;

        attackRate.value = config.attackRate.value;
        attackRate.limit = config.attackRate.limit;
        attackRate.isRate = true;

        attackRange.value = config.attackRange.value;
        attackRange.limit = config.attackRange.limit;
        attackRange.isRate = false;

        health.value = config.health.value;
        health.limit = config.health.limit;
        health.isRate = false;

        damage.value = config.damage.value;
        damage.limit = config.damage.limit;
        damage.isRate = false;

        killValue = config.killValue;
    }

    public void Upgrade(float scale)
    {
        health.value += health.limit * scale * 3;
        //attackRate.value -= attackRate.limit * scale;
        damage.value += damage.limit * scale;
        //speed.value += speed.limit * scale;
    }

    public void Revert()
    {
        health.value = health.limit;
        attackRate.value = attackRate.limit;
        damage.value = damage.limit;
        speed.value = speed.limit;
    }
}
