using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WWP.Game
{
    public class EndGamePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _resultTextWin;
        [SerializeField] private TextMeshProUGUI _resultTextLose;
        [SerializeField] private Button _restartWin;
        [SerializeField] private Button _restartLose;
        [SerializeField] private Button _quitWin;
        [SerializeField] private Button _quitLose;
        [SerializeField] private GameObject _win;
        [SerializeField] private GameObject _lose;
        private GameManager _gameManager;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            _restartWin.onClick.AddListener(_gameManager.RestartGame);
            _restartLose.onClick.AddListener(_gameManager.RestartGame);
            _quitWin.onClick.AddListener(_gameManager.OpenMenu);
            _quitLose.onClick.AddListener(_gameManager.OpenMenu);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(GameManager.EndGameInfo info)
        {
            gameObject.SetActive(true);
            if (info.win)
            {
                _lose.SetActive(false);
                _win.SetActive(true);
                _resultTextWin.text = $"Level {info.level} completed";
            }
            else
            {
                _lose.SetActive(true);
                _win.SetActive(false);
                _resultTextLose.text = $"Level {info.level} is not completed";
            }
        }
    }
}
