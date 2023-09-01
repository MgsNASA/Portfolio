using UnityEngine;
using UnityEngine.UI;

public class ChangeAviatorImage : MonoBehaviour {
    public static ChangeAviatorImage Instance {
        get; private set;
    } // ��������

    public Button leftButton;
    public Button rightButton;
    public GameObject [] shopObjects; // ������ �������� ��������
    public GameObject [] aviators;

    private int currentIndex = 0; // ������� ������ ���������� �������

    private void Awake ( ) {
        if ( Instance == null ) {
            Instance = this; // ������������� ������������ ���������
        } else {
            Destroy (gameObject); // ���� ��������� ��� ����������, ���������� ����� ������
            return;
        }
    }

    private void Start ( ) {
        leftButton.onClick.AddListener (ShowPreviousObject);
        rightButton.onClick.AddListener (ShowNextObject);

        // �������� ������ ������
        ShowObject (currentIndex);
    }

    public void ShowObject ( int index ) {
        // �������� ������ ��������� ������, ��������� ���������
        for ( int i = 0; i < shopObjects.Length; i++ ) {
            shopObjects [i].SetActive (i == index);
        }

        // ��������� ��������� �������������� ���������
        bool anyActivated = false;
        for ( int i = 0; i < aviators.Length; i++ ) {
            bool isActivated = AviatorManager.Instance.aviators [i].isPurchased && i == index;
            aviators [i].SetActive (isActivated);
            if ( isActivated ) {
                anyActivated = true;
            }
        }

        // ���� �� ���� ������� �� �����������, ���������� ������ ���������
        if ( !anyActivated ) {
            for ( int i = 0; i < aviators.Length; i++ ) {
                if ( AviatorManager.Instance.aviators [i].isPurchased ) {
                    aviators [i].SetActive (true);
                    break;
                }
            }
        }
    }

    private void ShowPreviousObject ( ) {
        currentIndex--;
        if ( currentIndex < 0 ) {
            currentIndex = shopObjects.Length - 1;
        }
        ShowObject (currentIndex);
    }

    private void ShowNextObject ( ) {
        currentIndex++;
        if ( currentIndex >= shopObjects.Length ) {
            currentIndex = 0;
        }
        ShowObject (currentIndex);
    }
}
