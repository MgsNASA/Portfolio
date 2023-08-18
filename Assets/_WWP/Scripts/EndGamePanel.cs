using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WWP.Game
{
    public class EndGamePanel : MonoBehaviour
    {
        [SerializeField] private Button _restart;
        [SerializeField] private TextMeshProUGUI _result;
        [SerializeField] private TextMeshProUGUI _restartText;
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
            _result.text = info.win ? "YOU WIN" : "YOU DIDN'T FIND IT";
            _restartText.text = info.win ? "NEXT LEVEL" : "TRY AGAIN";
        }
    }
}
