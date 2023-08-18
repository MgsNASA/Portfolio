using UnityEngine;
using UnityEngine.UI;

namespace WWP.Game
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] protected float _progressBarMinWidth = 76f;
        [SerializeField] protected float _progressBarMaxWidth;
        [SerializeField] protected Image _progressBarFG;
        [SerializeField, Range(0f, 1f)] private float _value;
        public float Value { get => _value; set => SetValue(value); }

        protected virtual void SetValue(float value)
        {
            value = Mathf.Clamp01(value);
            _value = value;
            float newWidth = value * (_progressBarMaxWidth - _progressBarMinWidth) + _progressBarMinWidth;
            Vector2 size = _progressBarFG.rectTransform.sizeDelta;
            size.x = newWidth;
            _progressBarFG.rectTransform.sizeDelta = size;
            float newPosX = - (_progressBarMaxWidth - newWidth) / 2;
            _progressBarFG.transform.localPosition = new Vector3(newPosX, 0, 0);
        }

        protected virtual void OnValidate()
        {
            if (Application.isPlaying) return;
            SetValue(_value);
        }
    }
}
