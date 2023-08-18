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
        private SettingsMenu _settingsMenu;
        [SerializeField] private Button _restart;
        [SerializeField] private BallGame _ballGame;
        private static bool _setRotation = false;

        private float _timeStart;
        public bool Paused { get; private set; }
        public BallGame BallGame { get { return _ballGame; } }
        [SerializeField] private GameObject _menu;

        private void Start()
        {
             Application.targetFrameRate = 60;
            _restart.onClick.AddListener(() => RestartGame());
            _endGamePanel = FindObjectOfType<EndGamePanel>(true);
            _settingsMenu = FindObjectOfType<SettingsMenu> (true);
            _endGamePanel.Init(this);
            _settingsMenu.Init(this);
        }

        public void OpenMenu()
        {
            _menu.SetActive(true);
        }

        public void StartGame(Action turnOffLoadingScreen)
        {
            //_round++;
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
            OpenMenu();
            _timeStart = Time.time;
            _ballGame.Init(this);
            turnOffLoadingScreen?.Invoke();
        }

        /*private IEnumerator RunTimer()
        {
            int round = _round;
            yield return new WaitForSeconds(_roundTime);
            EndGame(0, new EndGameInfo
            {
                win = false,
                round = round,
            });
        }*/

        public void RestartGame()
        {
            StopAllCoroutines();
            var restartables = FindObjectsOfType<MonoBehaviour>(true).OfType<IRestartable>();
            foreach (var restartable in restartables)
            {
                restartable.OnRestart();
            }
            Ball [] cells = FindObjectsOfType<Ball> ();
            foreach ( Ball cell in cells ) {
                Destroy (cell.gameObject);
            }
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
            public int level;
        }
    }
}
