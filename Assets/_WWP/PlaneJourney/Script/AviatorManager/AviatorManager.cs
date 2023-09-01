using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class Aviator {
    public GameObject aviatorPrefab; // ������ ��������
    public int price; // ���� ��������
    public bool isPurchased = false; // ����, �����������, ������ �� �������
    public int airportIndex;
    public bool isActive = false; // ����, �����������, �������� �� ������� ��������
}


public class AviatorManager : MonoBehaviour {
    private static AviatorManager m_Instance;

    public static AviatorManager Instance {
        get {
            return m_Instance;
        }
    }

    public Aviator [] aviators; // ������ � ����������� � ���������
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
        // ��������� ��������� ��������� ���������
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
                // ��������� ��������� ��������� ���������
                SavePurchasedAviators ();

                image.gameObject.SetActive (false); // �������� ����������� ����� �������
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

        // ���������, ��� ����������� ���� �� ���� �������, ���� ���� ��������� ��������
        bool anyPurchased = false;
        for ( int i = 0; i < aviators.Length; i++ ) {
            if ( aviators [i].isPurchased ) {
                anyPurchased = true;
                aviators [i].isActive = true;
                break;
            }
        }

        // ���� �� ���� ������� �� ����������� � ���� ��������� ��������, ���������� ������
        if ( !anyPurchased && aviators.Length > 0 ) {
            aviators [0].isActive = true;
        }

        SavePurchasedAviators (); // ��������� ���������
    }

}
