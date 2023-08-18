using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] private Transform _enemySpawnPoint;

    [SerializeField] private float _timeToSpawnEnemy = 2f;
    [SerializeField] private float _timeToSpawnModified;
    private float _defaultSpawnRate;

    private float _timeSinceSpawn;

    private EnemyConfig _enemyConfig;

    public EnemyConfig EnemyConfig { set { _enemyConfig = value; } }

    [SerializeField] private float _maxTime = 60;
    [SerializeField] private float _maxRate = 2f;
    private float _startTime;

    public bool Active { get; set; }

    private void Start()
    {
        _defaultSpawnRate = _timeToSpawnEnemy;
    }

    public void Restart()
    {
        Active = true;
        _startTime = Time.time;
        _timeSinceSpawn = _timeToSpawnEnemy;
        _timeToSpawnModified = _timeToSpawnEnemy;
        _enemyConfig.Revert();
        _timeToSpawnEnemy = _defaultSpawnRate;
    }

    private void Update()
    {
        if (!Active) return;

        _timeSinceSpawn += Time.deltaTime;

        if (_timeSinceSpawn > _timeToSpawnModified)
        {
            _timeSinceSpawn = 0f;
            SlimeShooterManager.instance.OnEnemySpawn(SpawnEnemy());
        }
    }

    private EnemyModel SpawnEnemy()
    {
        float offset = Random.Range(0f, 5f);
        _enemyConfig.Upgrade(0.1f);

        float timeElapsed = Time.time - _startTime;
        if (timeElapsed > _maxTime) timeElapsed = _maxTime;
        float bT = _timeToSpawnEnemy;
        float mR = _maxRate;
        float mT = _maxTime;
        float mod = (mR - bT) / mT * timeElapsed + bT;
        _timeToSpawnModified = mod;
        return new EnemyModel(_enemyConfig, _enemySpawnPoint, offset);
    }
}
