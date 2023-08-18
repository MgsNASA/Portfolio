using UnityEngine;
using UnityEngine.UI;

namespace WWP.Game
{
    public class ProgressBarSquare : MonoBehaviour
    {
        [SerializeField] private Image _fg;

        public void SetValue(float value)
        {
            _fg.fillAmount = value;
        }
    }
}
