using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUIInteraction : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI Money;
    [Header ("UiElementsAviator2")]
    [SerializeField]
    private Button buttonAviator2;
    [SerializeField]
    private Image _imageAviator2; // Изображение для самолета 2
    [Header ("UiElementsAviator3")]
    [SerializeField]
    private Button buttonAviator3;
    [SerializeField]
    private Image _imageAviator3; // Изображение для самолета 3

    private AviatorManager m_AviatorManager;

    private void Start ( ) {
        m_AviatorManager = AviatorManager.Instance;
        Money.text = MoneyManager.instance.GetMoney ().ToString ();

        // Проверяем состояние купленности и скрываем изображения для купленных самолетов
        for ( int i = 0; i < m_AviatorManager.aviators.Length; i++ ) {
            Aviator aviator = m_AviatorManager.aviators [i];
            Image image = null;

            if ( i == 1 ) {
                image = _imageAviator2;
            } else if ( i == 2 ) {
                image = _imageAviator3;
            }

            if ( aviator.isPurchased && image != null ) {
                image.gameObject.SetActive (false); // Скрываем изображение для купленного самолета
            }
        }

        buttonAviator2.onClick.AddListener (( ) => TryPurchaseAviator (1, _imageAviator2));
        buttonAviator3.onClick.AddListener (( ) => TryPurchaseAviator (2, _imageAviator3));
    }
    public void ChangeMoney ( int amount ) {
        Money.text = amount.ToString ();
        Debug.Log ("Money changed: " + amount);
    }
    private void TryPurchaseAviator ( int aviatorIndex, Image image ) {
        m_AviatorManager.TryPurchaseAviator (aviatorIndex, image);
    }

   
}
