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

   

    // ������ �������� gameManager, ��� ��� �� ��������������� � Init
    public void Init ( GameManager gameManager ) {
        _gameManager = gameManager;
        _callingMenuButton.onClick.AddListener (ToggleSettingsPanel);
        _audioButton.onClick.AddListener (ToggleSoundState);

    }

    // ������� �������� ������, ����� ���� ����� �������, ��� �� ������
    public void ToggleSettingsPanel ( ) {
        _gameManager.TogglePause (!_gameManager.Paused); // ����������� ��������� �����

        if ( _settingsMenu.activeSelf ) {
            _settingsMenu.SetActive (false); // ��������� ������ ��������
        } else {
            _settingsMenu.SetActive (true); // ��������� ������ ��������
        }
    }


    public void ToggleSoundState ( ) {
        Debug.Log ("ToggleSoundState() called"); // ���������, ��������� �� ��� ���������

        _audioListener.enabled = !_audioListener.enabled;

        ColorBlock colors = _audioButton.colors;

        if ( _audioListener.enabled ) {
            // ���������� ���������
            colors.normalColor = new Color (1f, 1f, 1f); // ����� ���� (�������)
        } else {
            // ����������� ���������
            colors.normalColor = new Color (0.2f, 0.2f, 0.2f); // �����-����� ���� (������)
        }

        _audioButton.colors = colors;

        Debug.Log ("Sound enabled: " + _audioListener.enabled);
    }


}
