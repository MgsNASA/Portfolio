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

        [SerializeField] private int _roundTime = 90;
        [SerializeField] private UI_Timer _timerUI;

        public float _timeStart;
        public Coroutine _timer;
        [SerializeField] private FinderGameManager _finderGame;
        public bool Paused { get; private set; }

        private IEnumerator Start()
        {
            yield return null;
            Application.targetFrameRate = 60;
            _restart.onClick.AddListener(() => RestartGame());
            _endGamePanel = FindObjectOfType<EndGamePanel>(true);
            _endGamePanel.Init(this);
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
            _timeStart = Time.time;
            _timer = StartCoroutine(RunTimer());
            _timerUI.RunTimer(_roundTime);
            _finderGame.Init();
            turnOffLoadingScreen?.Invoke();
        }

        private IEnumerator RunTimer()
        {
            yield return new WaitForSeconds(_roundTime);
            EndGame(0, new EndGameInfo
            {
                win = false
            });
        }

        public void RestartGame()
        {
            StopCoroutine(_timer);
            _timerUI.StopTimer();
            var restartables = FindObjectsOfType<MonoBehaviour>(true).OfType<IRestartable>();
            foreach (var restartable in restartables)
            {
                restartable.OnRestart();
            }
            StartGame(null);
        }
        
        public void EndGame(float delay, EndGameInfo info)
        {
            StopCoroutine(_timer);
            _timerUI.StopTimer();
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
            public int level;
        }
        public void resetTimer ( ) {
            _timerUI.StopTimer ();
            _timeStart = Time.time;
            _timer = StartCoroutine (RunTimer ());
            _timerUI.RunTimer (_roundTime);
        }
    }
}
