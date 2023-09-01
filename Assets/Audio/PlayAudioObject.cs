using UnityEngine;
using UnityEngine.UI;

namespace GooyesPlugin
{
    public class PlayAudioObject : MonoBehaviour
    {
        public string _soundToPlayOnEnable;
        public string _soundToPlayOnDisable;
        public string _soundToPlayOnClick;

        public Audio.SFXType type = Audio.SFXType.INGAME;

        public bool onScreenOnly = false;
        public bool playAudioOnButtonClick;
        public bool vibrateOnButtonClick = true;
        private Button _button;

        private void OnEnable()
        {
            if (playAudioOnButtonClick)
            {
                _button = GetComponent<Button>();
                if (_button == null) Debug.LogError("Missing Button component", gameObject);
            }
            if (playAudioOnButtonClick && !string.IsNullOrEmpty(_soundToPlayOnClick))
            {
                _button.onClick.AddListener(OnButtonClick);
            }

            Play(_soundToPlayOnEnable);
        }

        private void OnDisable()
        {
            if (playAudioOnButtonClick)
            {
                _button.onClick.RemoveListener(OnButtonClick);
            }

            Play(_soundToPlayOnDisable);
        }

        private void OnButtonClick()
        {
            Play(_soundToPlayOnClick);
        }


        private void Play(string soundName)
        {
            bool hasAudio = !string.IsNullOrEmpty(soundName);
            if (onScreenOnly)
            {
             //   hasAudio = Camera.main.IsOnScreen(transform.position);
            }
            if (hasAudio)
            {
                Audio.Get().PlaySound(soundName, type);
                //if (vibrateOnButtonClick) Audio.Get().Vibrate(100);
            }
        }
    }
}
