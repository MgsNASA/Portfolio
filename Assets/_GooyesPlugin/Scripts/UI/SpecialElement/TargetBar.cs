using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using WWP.Game;

namespace GooyesPlugin.UI
{
    internal class TargetBar : ProgressBar
    {
        [SerializeField, Range(0f, 1f)] private float _targetValue;
        [SerializeField] private Image _targetZone;
        [SerializeField] private RectTransform _targetObject;
        //[SerializeField] private RectTransform _targetBorder;
        //[SerializeField] private RectTransform _targetParent;
        //[SerializeField] private RectTransform _referenceBorder;
        public float TargetValue { get { return _targetValue; } set { SetTargetValue(value); } }

        private void SetTargetValue(float value)
        {
            value = Mathf.Clamp01(value);
            _targetValue = value;

            float min = _progressBarMinWidth;
            float max = _progressBarMaxWidth;
            float x = (value - 0.5f) * (max - min);

            float newWidth = max - (x + max / 2);
            float newPosX = (_progressBarMaxWidth - newWidth) / 2;

            _targetZone.rectTransform.sizeDelta = new Vector2(newWidth, _targetZone.rectTransform.sizeDelta.y);
            _targetZone.transform.localPosition = new Vector3(newPosX, 0, 0);

            _targetObject.localPosition = new Vector3(x, 0, 0);

            if (Application.isPlaying)
            {
                StartCoroutine(SetTarget());
            }
            else
            {
                //_targetZone.transform.localPosition = 
                //_targetBorder.position = _referenceBorder.position;
                //_targetParent.sizeDelta = _progressBarFG.rectTransform.sizeDelta;
            }
        }

        private IEnumerator SetTarget()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            //_targetObject.position = _progressBarFG.rectTransform.sizeDelta;
            //_targetBorder.position = _referenceBorder.position;
            //_targetParent.sizeDelta = _progressBarFG.rectTransform.sizeDelta;
        }

        protected override void OnValidate()
        {
            if (Application.isPlaying) return;
            base.OnValidate();
            SetTargetValue(_targetValue);
        }
    }
}
