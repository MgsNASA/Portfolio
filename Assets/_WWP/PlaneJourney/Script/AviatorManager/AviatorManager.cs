using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class Aviator {
    public GameObject aviatorPrefab; // Префаб самолета
    public int price; // Цена самолета
    public bool isPurchased = false; // Флаг, указывающий, куплен ли самолет
    public int airportIndex;
    public bool isActive = false; // Флаг, указывающий, является ли самолет активным
}


public class AviatorManager : MonoBehaviour {
    private static AviatorManager m_Instance;

    public static AviatorManager Instance {
        get {
            return m_Instance;
        }
    }

    public Aviator [] aviators; // Массив с информацией о самолетах
    [SerializeField]
    private MoneyManager moneyManager = MoneyManager.instance;

    private void Awake ( ) {
        if ( m_Instance == null ) {
            m_Instance = this;
        } else {
            Destroy (gameObject);
        }
    }

    private void Start ( ) {
        // Загружаем состояние купленных самолетов
        LoadPurchasedAviators ();
        //ResetAviators ();
        aviators[0].isPurchased = true;
    }

    public void TryPurchaseAviator ( int aviatorIndex, Image image ) {
        if ( aviatorIndex >= 0 && aviatorIndex < aviators.Length ) {
            Aviator aviator = aviators [aviatorIndex];

            if ( !aviator.isPurchased && moneyManager.GetMoney () >= aviator.price ) {
                moneyManager.SubtractMoney (aviator.price);
                aviator.isPurchased = true;
                ChangeAviatorImage.Instance.ShowObject (aviatorIndex);
                // Сохранить состояние купленных самолетов
                SavePurchasedAviators ();

                image.gameObject.SetActive (false); // Скрываем изображение после покупки
                Debug.Log ("Purchased aviator: " + aviatorIndex);
            } else {
                Debug.Log ("Cannot purchase aviator: " + aviatorIndex);
            }
        }
    }

    private void LoadPurchasedAviators ( ) {
        for ( int i = 0; i < aviators.Length; i++ ) {
            int purchasedValue = PlayerPrefs.GetInt ("AviatorPurchased_" + i, 0);
            aviators [i].isPurchased = purchasedValue == 1 ? true : false;
        }
    }

    private void SavePurchasedAviators ( ) {
        for ( int i = 0; i < aviators.Length; i++ ) {
            PlayerPrefs.SetInt ("AviatorPurchased_" + i, aviators [i].isPurchased ? 1 : 0);
        }
        PlayerPrefs.Save ();
    }
    public void ResetAviators ( ) {
        foreach ( var aviator in aviators ) {
            aviator.isPurchased = false;
            aviator.isActive = false;
        }

        // Убедитесь, что активирован хотя бы один самолет, если есть купленные самолеты
        bool anyPurchased = false;
        for ( int i = 0; i < aviators.Length; i++ ) {
            if ( aviators [i].isPurchased ) {
                anyPurchased = true;
                aviators [i].isActive = true;
                break;
            }
        }

        // Если ни один самолет не активирован и есть купленные самолеты, активируем первый
        if ( !anyPurchased && aviators.Length > 0 ) {
            aviators [0].isActive = true;
        }

        SavePurchasedAviators (); // Сохраняем состояние
    }

}
