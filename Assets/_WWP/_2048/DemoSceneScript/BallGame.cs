using System.Collections;
using UnityEngine;
using WWP;
using WWP.Game;

public class BallGame : MonoBehaviour
{
    public static BallGame Instance { get; private set; }

    [SerializeField] private Ball _ballPrefab;
    [SerializeField] private Ball[] _startBalls;
    [SerializeField] private Transform _throwPoint;
    [SerializeField] private ScoreControler _scoreControler;
    [SerializeField] private float _throwForce = 3;
    public ScoreControler ScoreControler { get => _scoreControler; }

    private Ball _currentBall;
    private GameManager _gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Init(GameManager gameManager)
    {
        StopAllCoroutines();
        if (_currentBall != null)
        {
            Destroy(_currentBall.gameObject);
        }
        _gameManager = gameManager;
        foreach (Ball startBall in _startBalls)
        {
            startBall.Init(Random.Range(1, 6), gameManager, true);
        }
        CreateEdgeColliders();
        Ball cell = GenerateRandomBall();
        _currentBall = cell;
        _scoreControler.Init(gameManager);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (_currentBall != null)
            {
                Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - _throwPoint.position).normalized;
                _currentBall.RigidBody.simulated = true;
                _currentBall.RigidBody.velocity = direction * _throwForce;
                _currentBall = null;
                StartCoroutine(GenerateNewBall());
            }
        }
    }

    public IEnumerator GenerateNewBall()
    {
        yield return new WaitForSecondsRealtime(1f);
        Ball cell = GenerateRandomBall();
        _currentBall = cell;
    }

    public Ball GenerateRandomBall()
    {
        Ball cell = Instantiate(_ballPrefab, _throwPoint.position, Quaternion.identity);
        int value = Random.Range(1, 4);
        cell.Init(value, _gameManager, false);
        cell.transform.position = _throwPoint.position;
        cell.RefreshVisual();
        return cell;
    }

    public void GenerateIncreasedBall(Ball original, Vector3 position, Vector2 velocity)
    {
        Ball cell = Instantiate(_ballPrefab, position, Quaternion.identity);
        Rigidbody2D newRigidbody = cell.GetComponent<Rigidbody2D>();
        if (cell.Value < 11)
        {
            if (newRigidbody != null)
            {
                newRigidbody.simulated = true;
                newRigidbody.AddForce(velocity, ForceMode2D.Impulse);
            }

            cell.Init(original.Value + 1, _gameManager, true);
        }
    }

    private void CreateEdgeColliders()
    {
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Camera.main.aspect;

        CreateCollider(new Vector2(0, screenHeight / 2 + 0.5f), new Vector2(screenWidth, 1), "TopCollider"); // Top collider
        CreateCollider(new Vector2(0, -screenHeight / 2 - 0.5f), new Vector2(screenWidth, 1), "BottomCollider");
        CreateCollider(new Vector2(-screenWidth / 2 - 0.5f, 0), new Vector2(1, screenHeight), "LeftCollider"); // Left collider
        CreateCollider(new Vector2(screenWidth / 2 + 0.5f, 0), new Vector2(1, screenHeight), "RightCollider"); // Right collider
    }

    private void CreateCollider(Vector2 position, Vector2 size, string name)
    {
        GameObject colliderObj = new GameObject(name);
        colliderObj.transform.position = position;

        BoxCollider2D collider = colliderObj.AddComponent<BoxCollider2D>();
        collider.size = size;
    }
}