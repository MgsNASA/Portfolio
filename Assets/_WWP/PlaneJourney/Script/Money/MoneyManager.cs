using UnityEngine;

public class MoneyManager : MonoBehaviour {
    public static MoneyManager instance;

    public int money = 0; // Количество денег

    private const string MoneyKey = "Money";
    [SerializeField]
    private MoneyUIInteraction MoneyUIInteraction;

    private void Awake ( ) {
        if ( instance == null ) {
            instance = this;
        } else {
            Destroy (gameObject);
        }
    }

    private void Start ( ) {
        LoadMoney ();
    }

    private void OnDestroy ( ) {
        SaveMoney ();
    }

    public void AddMoney ( int amount ) {
        money += amount;
        MoneyUIInteraction.ChangeMoney (money);
        SaveMoney ();
        Debug.Log ("LOLSD");
    }

    public void SubtractMoney ( int amount ) {
        if ( money >= amount ) {
            money -= amount;
            MoneyUIInteraction.ChangeMoney (money);
            SaveMoney ();
            Debug.Log ("ASDS");
        }
    }


    public int GetMoney ( ) {
        return money;
    }

    public static MoneyManager GetInstance ( ) {
        return instance;
    }

    private void SaveMoney ( ) {
        PlayerPrefs.SetInt (MoneyKey, money);
        PlayerPrefs.Save ();
    }

    private void LoadMoney ( ) {
        if ( PlayerPrefs.HasKey (MoneyKey) ) {
            money = PlayerPrefs.GetInt (MoneyKey);
        }
    }
}
