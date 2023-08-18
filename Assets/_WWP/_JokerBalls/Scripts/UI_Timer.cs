using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace WWP.Game
{
    public class UI_Timer : MonoBehaviour
    {
        private CancellationTokenSource _token;
        [SerializeField] private TextMeshProUGUI _timeText;
        private bool _stopped;
        public void RunTimer(int time)
        {
            _stopped = false;
            _token = new CancellationTokenSource();
            TimerRoutine(time).Forget();
        }
        private async UniTask TimerRoutine(int time)
        {
            time--;
            while (time > 0 && !_stopped)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(time);
                string str = timeSpan.ToString(@"mm\:ss");
                _timeText.text = str;
                time--;
                await UniTask.Delay(1000, false, PlayerLoopTiming.Update, cancellationToken: _token.Token);
            }
        }
        public void StopTimer()
        {
            _token.Cancel();
            _token.Dispose();
            _stopped = true;
            _token = new CancellationTokenSource();
        }
    }
}