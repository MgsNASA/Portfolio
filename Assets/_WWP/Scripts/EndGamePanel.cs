using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WWP.Game
{
    public class EndGamePanel : MonoBehaviour
    {
        //[SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private Button _restart;
        private GameManager _gameManager;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _restart.onClick.AddListener(() => _gameManager.RestartGame());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(GameManager.EndGameInfo info)
        {
            gameObject.SetActive(true);

        }
    }
}
