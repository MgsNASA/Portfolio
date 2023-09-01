using NSubstitute.Core;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clicker : MonoBehaviour {
    public TextMeshProUGUI clickCountText;
    public TextMeshProUGUI clickSpeedText;
    public TextMeshProUGUI MoneyHuman;
    public Button clickButton;
    public Image buttonImage; // ������ �� ��������� Image ������
    [SerializeField]
    private long clickSpeed = 0; // ��������� ���������� ������ � �������, �������� �� ��� ulong
    [SerializeField]
    private long money = 0; // �������� �� ��� ulong
    public bool isPassiveIncomeEnabled = false; // ���� ��� ����������/���������� ����������� ����� ������ �������
    [SerializeField]
    private int tapsCount = 0; // ��� int ��������, ��� ��� ��� ���������� ������, ������� ������������ � ��� int
    [SerializeField]
    private bool isButtonPurchased = false; // ���������� ��� ������������ ��������� ������� ������

    [SerializeField]
    private TextMeshProUGUI autominingButtonText;

    public void StartClass ( ) {
        if ( tapsCount >= 100 ) {
            isButtonPurchased = true;
            UpdateButtonImage ();
        }
       //ResetData ();
        clickButton.onClick.AddListener (OnClickButton);

        InvokeRepeating ("IncreaseMoney", 1f, 1f);
        LoadData ();
        UpdateUI ();
        UpdateButtonImage (); // ��������� ����� ������ UpdateButtonImage() �����
    }

    public void OnClickButton ( ) {
        money+= clickSpeed; // ����� �������������� ����� � ulong
        tapsCount++;
        UpdateUI ();
        

        }
        public void MINUSMoney ( float  cost ) {

        money -= ( long ) cost; // ���������� ����� ����� �������
        SaveData ();
        UpdateUI ();
    }
    

    private void IncreaseMoney ( ) {
        if ( isPassiveIncomeEnabled ) {
            money += clickSpeed;
            UpdateUI ();
            SaveData ();
        }
    }

    public long Money {
        get {
            return money; // ������ ���������� ������� �������� ����� (��� ulong).
        }
        set {
            money = value; // ������ ������������, ��� ��������������
            UpdateUI (); // ����� ������ UpdateUI() ��� ���������� ����������������� ����������.
        }
    }

    public long ClickSpeed {
        get {
            return clickSpeed;
        }
        set {
            clickSpeed = value; // ������ ������������, ��� ��������������
            UpdateUI ();
        }
    }

    public void ApplyPassiveIncome ( int passiveIncome ) {
        money += ( long ) passiveIncome; // ����� �������������� �����
        SaveData ();
        UpdateUI ();
    }

    private void UpdateUI ( ) {
        UpdateUINormal ();
        UpdateUIHuman ();
        UpdateUIFormatted ();
    }

    private void UpdateUINormal ( ) {
        float moneyInMillions = money / 1000000000f;
        clickCountText.text = moneyInMillions.ToString ("F9") + "B";
    }

    private void UpdateUIHuman ( ) {

        MoneyHuman.text = money.ToString () + " " + "Satoshi";
    }

    private void UpdateUIFormatted ( ) {
        clickSpeedText.text = clickSpeed.ToString ("0.##########") + " " + "Satoshi/sec";
    }

    private string GetFormattedMoneyString ( int value ) {
        string [] numberSuffixes = { "", " thousand", " million", " billion", " trillion", " quadrillion", " quintillion", " sextillion", " septillion", " octillion", " nonillion", " decillion", " undecillion", " duodecillion", " tredecillion", " quattuordecillion", " quindecillion", " sexdecillion", " septendecillion", " octodecillion", " novemdecillion", " vigintillion" };

        int suffixIndex = 0;
        while ( value >= 1000 ) {
            value /= 1000;
            suffixIndex++;
        }

        return value.ToString () + numberSuffixes [suffixIndex];
    }

    private void SaveData ( ) {
        PlayerPrefs.SetString ("Money", money.ToString ());
        PlayerPrefs.SetString ("ClickSpeed", clickSpeed.ToString ()); // �������������� � ������ � ����������
        PlayerPrefs.SetInt ("TapsCount", tapsCount);
        PlayerPrefs.SetInt ("IsButtonPurchased", isButtonPurchased ? 1 : 0);
    }

    private void LoadData ( ) {
        string moneyString = PlayerPrefs.GetString ("Money", "1");
        if ( long.TryParse (moneyString, out long moneyValue) ) {
            money = moneyValue;
        } else {
            money = 0;
        }

        string clickSpeedString = PlayerPrefs.GetString ("ClickSpeed", "1");
        if ( long.TryParse (clickSpeedString, out long clickSpeedValue) ) {
            clickSpeed = clickSpeedValue;
        } else {
            clickSpeed = 0;
        }

        tapsCount = PlayerPrefs.GetInt ("TapsCount", 0);
        isButtonPurchased = PlayerPrefs.GetInt ("IsButtonPurchased", 0) == 1;
    }

    public void EnablePassiveIncome ( ) {
        if ( tapsCount >= 99 ) {
            if ( !isButtonPurchased ) {
              
       
                // ��������� ��������� �������� ����� ���������� ������������
                Invoke ("ActivateAutoMining", 0.1f);
            } else {
                isPassiveIncomeEnabled = !isPassiveIncomeEnabled;
            }
        }
        UpdateButtonImage ();
    }

    public void UpdateButtonImage ( ) {
        Color buttonColor = buttonImage.color;
        if ( tapsCount >= 99 ) {
            if ( isButtonPurchased ) {
                autominingButtonText.text = "Automining";
            } else {
                autominingButtonText.text = "Cost 100";
            }

            if ( !isPassiveIncomeEnabled ) {
                buttonColor.a = 0.5f; // ���� ��������� ����� �������, ������ ������ ����� ������
            } else {
                buttonColor.a = 1f; // ���� ��������� ����� ��������, ������ ������ ����� �������
            }
        } else {
            autominingButtonText.text = "Cost 100";
        }
        buttonImage.color = buttonColor;
    }

    public void ResetData ( ) {
        // �������� ������ �� �������� �� ���������
        clickSpeed = 0;
        money = 0;

        SaveData ();
        UpdateUI ();
    }

    public void ActivateAutoMining ( ) {
        if ( isButtonPurchased == false ) {
            money = money - 100; // ������ ������������, ��� ��������������
            Debug.Log ("asdl");
            isButtonPurchased = true;
            isPassiveIncomeEnabled = true;
            UpdateUI ();
            UpdateButtonImage ();
        }
    }
}
