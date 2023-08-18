using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WWP;
using WWP.Game;

public class SlimeShooterManager : Singleton<SlimeShooterManager>
{
    private GameManager _gameManager;
    public Transform gameObjectContainer;
    private int _enemiesKilled;

    public Transform shootPoint;
    public Transform runPoint;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject slimePrefab;

    public EnemyConfig enemyConfig;
    public CharacterConfig playerConfig;
    public SlimeConfig slimeConfig;

    public EnhancementData enhancementData;

    private PlayerModel _player;
    private List<EnemyModel> _enemies;

    public PlayerModel Player { get { return _player; } }

    public event Action RestartEvent;

    private bool _isPlaiyng;
    private bool? _movingByButton = null;

    private void Start()
    {
        _enemies = new List<EnemyModel>();
        EnemySpawner.Instance.EnemyConfig = enemyConfig;
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (!_isPlaiyng) return;
        if (_player != null) _player.Update();
        foreach (EnemyModel enemy in _enemies)
        {
            enemy.Update();
        }
        if (_movingByButton.HasValue)
        {
            _player.MoveFromButton(_movingByButton.Value);
        }
    }

    public bool Enhance(Enhancement enhancement)
    {
        if (_player != null && _player.Active)
        {
            if (WalletManager.Instance.CheckMoney(enhancement.price))
            {
                WalletManager.Instance.Enhance(enhancement.price);
                _player.Enhance(enhancement);
                UIManager.Instance.UpdateUI(new UIUpdateData()
                {
                    playerAttackRate = _player.Config.attackRate.value,
                    damageMax = _player.Config.damage.value >= _player.Config.damage.limit,
                    playerDamage = _player.Config.damage.value,
                    arMax = _player.Config.attackRate.value <= _player.Config.attackRate.limit
                });
                return true;
            }
        }
        return false;
    }

    public void OnMoveButtonUp()
    {
        _movingByButton = null;
    }
    public void OnMoveButtonDown(bool up)
    {
        _movingByButton = up;
    }

    public void Restart()
    {
        _enemiesKilled = 0;
        _movingByButton = null;
        List<EnemyModel> enemies = new List<EnemyModel>();

        foreach (EnemyModel enemy in _enemies)
        {
            enemies.Add(enemy);
        }

        foreach(EnemyModel enemy in enemies)
        {
            enemy.Recycle(false);
        }

        _enemies = new List<EnemyModel>();

        if (_player != null)
        {
            _player.Died -= OnPlayerDeath;
            _player.Recycle();
        }
        _player = new PlayerModel(playerConfig);
        _player.Died += OnPlayerDeath;

        RestartEvent?.Invoke();
        EnemySpawner.Instance.Restart();
        WalletManager.Instance.Restart();
        EnhancementManager.Instance.Restart();
        UIManager.Instance.Restart();

        _isPlaiyng = true;
    }

    public void OnEnemySpawn(EnemyModel enemy)
    {
        _enemies.Add(enemy);
        enemy.Died += OnEnemyDeath;
    }

    private void OnEnemyDeath(Unit enemyUnit, bool inProcess)
    {
        EnemyModel enemy = enemyUnit as EnemyModel;
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
        }
        if (!inProcess) return;
        WalletManager.Instance.IncreaseMoneyAmout(enemy.Config.killValue);
        _enemiesKilled++;
        UIManager.Instance.UpdateUI();
    }

    private void OnPlayerDeath(Unit unit, bool died)
    {
        _isPlaiyng = false;
        EnemySpawner.Instance.Active = false;
        _gameManager.EndGame(0, new GameManager.EndGameInfo
        {
            win = false,
            value = _enemiesKilled
        });
    }

    public bool CheckEnemy()
    {
        return _enemies.Count > 0;
    }

    public EnemyModel GetClosestEnemy()
    {
        EnemyModel closestEnemy = null;
        if (_enemies.Count > 0) closestEnemy = _enemies[0];

        foreach (EnemyModel enemy in _enemies)
        {
            if (enemy.Transform.position.x < closestEnemy.Transform.position.x)
                closestEnemy = enemy;
        }

        return closestEnemy;
    }
}

public class Enhancement
{
    public string targetParameterName;
    public float value;
    public int price;
}

[Serializable]
public class Parameter
{
    public float value;
    public float limit;
    public bool isRate;
}

[Serializable]
public class CharacterConfig
{
    public Parameter speed;
    public Parameter attackRate;
    public Parameter attackRange;
    public Parameter health;
    public Parameter damage;

    public CharacterConfig()
    {

    }

    public CharacterConfig(CharacterConfig config)
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
    }
}

public struct HitConfig
{
    public float damage;
}


public class MyTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public MyTransform(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.localScale;
    }

    public MyTransform(MyTransform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.scale;
    }
}
