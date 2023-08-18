using TMPro;
using WWP.Game;
using UnityEngine;
using UnityEngine.UI;

namespace WWP
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _percent;
        [SerializeField] private ProgressBar _progressBar;
        //[SerializeField] private Image _circle;
        [SerializeField] private float _fillSpeed = 0.01f;
        public bool IsShowed { get => gameObject.activeSelf; }
        private float _targetValue;
        private float _currentValue;

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show(float value = 0)
        {
            gameObject.SetActive(true);
            _percent.text = ToPercent(value);
            //_circle.fillAmount = value;
            _progressBar.Value = value;
            _targetValue = value;
            _currentValue = value;
        }

        public void Update()
        {
            if (IsShowed)
            {
                _currentValue = Mathf.MoveTowards(_currentValue, _targetValue, _fillSpeed);
                _percent.text = ToPercent(_currentValue);
                _progressBar.Value = _currentValue;
                //_circle.fillAmount = _currentValue;
            }
        }

        public void SetLoadStatus(float valueFromZeroToOne)
        {
            valueFromZeroToOne = Mathf.Clamp01(valueFromZeroToOne);
            _targetValue = valueFromZeroToOne;
        }

        private string ToPercent(float value)
        {
            int percent = (int)(value * 100);
            return $"{percent}%";
        }
    }
}
