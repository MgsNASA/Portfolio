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
       // [SerializeField] private Button _restart;
        [SerializeField] Clicker clicker;
        [SerializeField] Upgrade [] upgrade;
        private static bool _setRotation = false;

        //[SerializeField] private Field _field;
        //[SerializeField] private int _roundTime = 90;
        //[SerializeField] private UI_Timer _timerUI;

        private float _timeStart;
        private Coroutine _timer;
        private int _round;
        public bool Paused { get; private set; }

        private void Start()
        {
            Application.targetFrameRate = 60;
          //  _restart.onClick.AddListener(() => RestartGame());
            //_endGamePanel = FindObjectOfType<EndGamePanel>(true);
            //_endGamePanel.Init(this);
        }

        public void StartGame(Action turnOffLoadingScreen)
        {
            Debug.Log("Start game");
            _round++;
            TogglePause(false);
            //_endGamePanel.Hide();
            //_scoreBar.Init(GetLevel()); // set score here
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
            clicker.StartClass ();
            for (int i = 0;i<upgrade.Length;i++ ) {
                upgrade[i].StartClass ();
            }
         
            // _field.Init(this);
            //  _timer = StartCoroutine(RunTimer());
            // _timerUI.RunTimer(_roundTime);

            turnOffLoadingScreen?.Invoke();
        }

       /* private IEnumerator RunTimer()
        {
            int round = _round;
            yield return new WaitForSeconds(_roundTime);
            EndGame(0, new EndGameInfo
            {
                win = false,
                round = round,
            });
        }
       */

        public void RestartGame()
        {
            var restartables = FindObjectsOfType<MonoBehaviour>(true).OfType<IRestartable>();
            foreach (var restartable in restartables)
            {
                restartable.OnRestart();
            }
            StartGame(null);
        }

        public void EndGame(float delay, EndGameInfo info)
        {
            if (info.round.HasValue && info.round.Value != _round) return;
            StopCoroutine(_timer);
            //_timerUI.StopTimer();
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
            public int? round;
        }
    }
}
