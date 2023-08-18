using TMPro;
using UnityEngine;
using WWP;
using DG.Tweening;
using GooyesPlugin;

public class Ball : MonoBehaviour
{
    public int Value { get; private set; }
    public int Points => isEmpty ? 0 : (int)Mathf.Pow(2, Value);
    public bool isEmpty => Value == 0;
    public const int maxValue = 11;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private TextMeshPro _valueText;
    [SerializeField] private bool hasMerged = false;
    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private LayerMask _deadLayer;
    private GameManager _gameManager;
    private bool _hadCollision;
    public Rigidbody2D RigidBody { get { return _rb; } }

    public void Init(int value, GameManager gameManager, bool hadCollision)
    {
        _hadCollision = hadCollision;
        _gameManager = gameManager;
        Value = value;
        RefreshVisual();
    }

    public void IncreaseValue()
    {
        RefreshVisual();
        _gameManager.BallGame.ScoreControler.AddPoints(Points);
    }

    public void Merge ( Ball other ) {
        if ( Value == other.Value && !hasMerged && !other.hasMerged ) {
            IncreaseValue ();
            hasMerged = true;
            other.hasMerged = true;

            Vector3 targetPosition = ( transform.position + other.transform.position ) / 2;

            // Создаем анимацию движения обоих шариков к центральной позиции
            Sequence mergeSequence = DOTween.Sequence ();
            mergeSequence.Append (transform.DOMove (targetPosition, 0.2f));
            mergeSequence.Join (other.transform.DOMove (targetPosition, 0.2f));

            // Создаем анимацию изменения масштаба (эффект желе)
            float initialScale = transform.localScale.x;
            float targetScale = initialScale * 1.1f; // Новый масштаб на 10% больше
            mergeSequence.Join (transform.DOScale (targetScale, 0.2f).SetEase (Ease.OutBack));
            mergeSequence.Join (other.transform.DOScale (targetScale, 0.2f).SetEase (Ease.OutBack));

            // Действия после анимации
            mergeSequence.OnComplete (( ) =>
            {
                // Генерируем новый шарик после анимации соединения
                Vector2 velocity = _rb.velocity + other._rb.velocity;
                velocity /= 2;
                if ( velocity.y < 0 )
                    velocity.y = -velocity.y;
                if ( velocity.magnitude > 3 ) {
                    velocity.Normalize ();
                    velocity *= 3;
                }
                BallGame.Instance.GenerateIncreasedBall (this, transform.position, velocity);
              //  Audio.Get ().Play ();
                // Уничтожаем оба шарика
                Destroy (other.gameObject);
                Destroy (gameObject);
            });
        }
    }

    public void RefreshVisual()
    {
        if (gameObject.activeSelf)
        {
            _valueText.text = isEmpty ? string.Empty : Points.ToString();
            _valueText.color = Value <= 2 ? ColorManager.Instance.PointsDarkColor : ColorManager.Instance.PointsLightColor;
            _sprite.sprite = ColorManager.Instance.sprites[Value];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_hadCollision) _hadCollision = true;
        if (collision.gameObject.CompareTag("Cell") && Value < 11)
        {
            Ball other = collision.gameObject.GetComponent<Ball>();
            if (other != null && other.Value == Value)
            {
                Merge(other);
            }
        }
        _collider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hadCollision && 1 << collision.gameObject.layer == _deadLayer)
        {
            _gameManager.EndGame(0, new GameManager.EndGameInfo
            {
                win = false,
                value = _gameManager.BallGame.ScoreControler.Points,
                level = _gameManager.BallGame.ScoreControler.Level
            });
        }
    }
}
