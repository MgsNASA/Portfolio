using System.Collections;
using UnityEngine;
using WWP;

public class Obstacle : MonoBehaviour {
    public int lifeTime = 5; // Время жизни препятствия
    [SerializeField]
    private bool _moneyOreObstacle;
    private GameManager _gameManager;
    public struct EndGameInfo {
        public bool win;
        public int value;
    }

    private void Awake ( ) {
        _gameManager = FindObjectOfType<GameManager> ();
        StartCoroutine (SelfDestroy(lifeTime));
    }
    private void OnTriggerEnter ( Collider other ) {
        if ( other.CompareTag ("Player") ) // Проверяем тег объекта, с которым столкнулись
        {

            if ( _moneyOreObstacle ) {  MoneyManager.instance.AddMoney (1); }
            else Debug.Log ("Player collided with obstacle!");
            if ( !_moneyOreObstacle )
                _gameManager.EndGame (0, new GameManager.EndGameInfo { win = true }); ;
            StartCoroutine (SelfDestroy (0));
        }
    }

    public IEnumerator SelfDestroy ( int lifetime) {
        // Ждем указанное время и удаляем препятствие
        yield return new WaitForSeconds (lifetime);
        Destroy (gameObject);
    }
}
