using UnityEngine;

namespace GooyesPlugin.UI
{
    internal class SpriteProgressBar : MonoBehaviour
    {
        [SerializeField] protected float _progressBarMaxWidth;
        [SerializeField] protected SpriteRenderer _progressBarFG;
        [SerializeField] protected SpriteRenderer _FGBorder;
        [SerializeField, Range(0f, 1f)] private float _value;
        public float Value { get => _value; set => SetValue(value); }

        protected virtual void SetValue(float value)
        {
            value = Mathf.Clamp01(value);
            _value = value;
            float newWidth = value * _progressBarMaxWidth;
            _progressBarFG.size = new Vector2(newWidth, _progressBarFG.size.y);
            _FGBorder.size = new Vector2(newWidth, _FGBorder.size.y);
        }

        protected virtual void OnValidate()
        {
            if (Application.isPlaying) return;
            //SetValue(_value);
        }
    }
}
