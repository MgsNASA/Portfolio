using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WWP;

public class SettingsMenu : MonoBehaviour {
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Button _callingMenuButton;
    [SerializeField] private Button _audioButton;
    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private AudioListener _audioListener;

   

    // Убрали параметр gameManager, так как он устанавливается в Init
    public void Init ( GameManager gameManager ) {
        _gameManager = gameManager;
        _callingMenuButton.onClick.AddListener (ToggleSettingsPanel);
        _audioButton.onClick.AddListener (ToggleSoundState);

    }

    // Изменим название метода, чтобы было более понятно, что он делает
    public void ToggleSettingsPanel ( ) {
        _gameManager.TogglePause (!_gameManager.Paused); // Инвертируем состояние паузы

        if ( _settingsMenu.activeSelf ) {
            _settingsMenu.SetActive (false); // Закрываем панель настроек
        } else {
            _settingsMenu.SetActive (true); // Открываем панель настроек
        }
    }


    public void ToggleSoundState ( ) {
        Debug.Log ("ToggleSoundState() called"); // Проверьте, выводится ли это сообщение

        _audioListener.enabled = !_audioListener.enabled;

        ColorBlock colors = _audioButton.colors;

        if ( _audioListener.enabled ) {
            // Включенное состояние
            colors.normalColor = new Color (1f, 1f, 1f); // Белый цвет (светлый)
        } else {
            // Выключенное состояние
            colors.normalColor = new Color (0.2f, 0.2f, 0.2f); // Темно-серый цвет (темный)
        }

        _audioButton.colors = colors;

        Debug.Log ("Sound enabled: " + _audioListener.enabled);
    }


}
