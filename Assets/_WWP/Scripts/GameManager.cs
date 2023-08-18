using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WWP.Game;

namespace WWP
{
    public class GameManager : MonoBehaviour
    {
        private EndGamePanel _endGamePanel;
        [SerializeField] private Button _restart;
        private static bool _setRotation = false;
        [SerializeField]
        private InfiniteRunner _infiniteRunner;
        [SerializeField]
        private ObstacleSpawner _obstacleSpawner;
        [SerializeField]
        private AviatorManager _aviatorManager;
        [SerializeField]
        private PlayerMovement [] _playerMovement;
        [SerializeField]
        private GameObject _uiStartWindow;
        [SerializeField]
        private Transform startPosition;
        [SerializeField]
        private Button _playGame;

        private float _timeStart;
        public bool Paused { get; private set; }

        private void Start()
        {
            Application.targetFrameRate = 60;
            _restart.onClick.AddListener(() => RestartGame());
            _playGame.onClick.AddListener(() => PlayGame());
            _endGamePanel = FindObjectOfType<EndGamePanel>(true);
            _endGamePanel.Init(this);
            _playerMovement = FindObjectsOfType<PlayerMovement> ();

        }

        public void StartGame(Action turnOffLoadingScreen)
        {
            TogglePause(false);
            _endGamePanel.Hide();
            if (!_setRotation)
            {
                Screen.orientation = ScreenOrientation.Portrait;
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = true;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = false;
                _setRotation = true;
            }
            AviatorManager.Instance.aviators [0].isPurchased = true;
            _timeStart = Time.time;
            turnOffLoadingScreen?.Invoke();
           
        }
        public void PlayGame ( ) {
            _infiniteRunner.StartCoroutine ("MoveLoop");
            _obstacleSpawner.StartCoroutine ("SpawnObjects");
            _uiStartWindow.SetActive (false);

            bool anyActivated = false; // Переменная для отслеживания активации хотя бы одного самолета

            for ( int i = 0; i < _aviatorManager.aviators.Length; i++ ) {
                if ( _aviatorManager.aviators [i].isPurchased ) {
                    _aviatorManager.aviators [i].isActive = true;
                    anyActivated = true;
                    break;
                }
            }

            // Если ни один самолет не активирован, активируем первый купленный
            if ( !anyActivated && _aviatorManager.aviators.Length > 0 ) {
                _aviatorManager.aviators [0].isActive = true;
            }

            // Включаем управление для всех игроков
            for ( int i = 0; i < _playerMovement.Length; i++ ) {
                _playerMovement [i].ChangeControlerMovement (true);
            }
        }



        public void RestartGame ( ) {
            // Сначала установите позицию игрока
            var playerMovements = FindObjectsOfType<PlayerMovement> ();
            foreach ( var playerMovement in playerMovements ) {
                playerMovement.transform.position =startPosition.position;
            }

            // Затем уничтожьте препятствия
            var obstacles = FindObjectsOfType<Obstacle> ();
            foreach ( var obstacle in obstacles ) {
                Destroy (obstacle.gameObject);
            }

            // После этого выполните остальные действия перезапуска
            var restartables = FindObjectsOfType<MonoBehaviour> (true).OfType<IRestartable> ();
            foreach ( var restartable in restartables ) {
                restartable.OnRestart ();
            }
            _infiniteRunner.gameObject.transform.position = new Vector3 (_infiniteRunner.gameObject.transform.position.x, 16.04f, _infiniteRunner.gameObject.transform.position.z);

            StartGame (null);
        }

        public void EndGame(float delay, EndGameInfo info)
        {
            StartCoroutine(EndGameDelay(delay, info));
        }

        private IEnumerator EndGameDelay(float delay, EndGameInfo info)
        {
            TogglePause(true);
            yield return new WaitForSecondsRealtime(delay);
            _endGamePanel.Show(info);
        }

        public void TogglePause(bool pause)
        {
            Time.timeScale = pause ? 0 : 1;
            Paused = pause;
        }

        public struct EndGameInfo
        {
            public bool win;
            public int value;
        }
    }
}
