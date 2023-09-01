using UnityEngine;
using UnityEngine.UI;

public class ChangeAviatorImage : MonoBehaviour {
    public static ChangeAviatorImage Instance {
        get; private set;
    } // Синглтон

    public Button leftButton;
    public Button rightButton;
    public GameObject [] shopObjects; // Массив объектов магазина
    public GameObject [] aviators;

    private int currentIndex = 0; // Текущий индекс выбранного объекта

    private void Awake ( ) {
        if ( Instance == null ) {
            Instance = this; // Устанавливаем единственный экземпляр
        } else {
            Destroy (gameObject); // Если экземпляр уже существует, уничтожаем новый объект
            return;
        }
    }

    private void Start ( ) {
        leftButton.onClick.AddListener (ShowPreviousObject);
        rightButton.onClick.AddListener (ShowNextObject);

        // Показать первый объект
        ShowObject (currentIndex);
    }

    public void ShowObject ( int index ) {
        // Включить только выбранный объект, остальные выключить
        for ( int i = 0; i < shopObjects.Length; i++ ) {
            shopObjects [i].SetActive (i == index);
        }

        // Проверяем состояние активированных самолетов
        bool anyActivated = false;
        for ( int i = 0; i < aviators.Length; i++ ) {
            bool isActivated = AviatorManager.Instance.aviators [i].isPurchased && i == index;
            aviators [i].SetActive (isActivated);
            if ( isActivated ) {
                anyActivated = true;
            }
        }

        // Если ни один самолет не активирован, активируем первый купленный
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
