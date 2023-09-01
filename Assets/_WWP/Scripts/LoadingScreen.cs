using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WWP.Game;

namespace WWP
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _percent;
        [SerializeField] private Image _circle;
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _percent.text = ToPercent(0);
            _circle.fillAmount = 0;
            //_progressBar.Value = 0;
        }

        public void SetLoadStatus(float percent)
        {
            percent = Mathf.Clamp01(percent);
            _percent.text = ToPercent(percent);
            _circle.fillAmount = percent;
            //_progressBar.Value = percent;
        }

        private string ToPercent(float value)
        {
            return $"{(int)(value * 100)}%";
        }
    }
}
