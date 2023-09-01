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
    public Image buttonImage; // Ссылка на компонент Image кнопки
    [SerializeField]
    private long clickSpeed = 0; // Начальное количество кликов в секунду, изменено на тип ulong
    [SerializeField]
    private long money = 0; // Изменено на тип ulong
    public bool isPassiveIncomeEnabled = false; // Флаг для разрешения/запрещения прибавления денег каждую секунду
    [SerializeField]
    private int tapsCount = 0; // Тип int оставлен, так как это количество кликов, которое укладывается в тип int
    [SerializeField]
    private bool isButtonPurchased = false; // Переменная для отслеживания состояния покупки кнопки

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
        UpdateButtonImage (); // Добавляем вызов метода UpdateButtonImage() здесь
    }

    public void OnClickButton ( ) {
        money+= clickSpeed; // Явное преобразование типов в ulong
        tapsCount++;
        UpdateUI ();
        

        }
        public void MINUSMoney ( float  cost ) {

        money -= ( long ) cost; // Приведение типов явным образом
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
            return money; // Геттер возвращает текущее значение денег (тип ulong).
        }
        set {
            money = value; // Прямое присваивание, без преобразований
            UpdateUI (); // Вызов метода UpdateUI() для обновления пользовательского интерфейса.
        }
    }

    public long ClickSpeed {
        get {
            return clickSpeed;
        }
        set {
            clickSpeed = value; // Прямое присваивание, без преобразований
            UpdateUI ();
        }
    }

    public void ApplyPassiveIncome ( int passiveIncome ) {
        money += ( long ) passiveIncome; // Явное преобразование типов
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
        PlayerPrefs.SetString ("ClickSpeed", clickSpeed.ToString ()); // Преобразование в строку и сохранение
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
              
       
                // Добавляем небольшую задержку перед активацией автомайнинга
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
                buttonColor.a = 0.5f; // Если пассивный доход включен, делаем кнопку более темной
            } else {
                buttonColor.a = 1f; // Если пассивный доход выключен, делаем кнопку более светлой
            }
        } else {
            autominingButtonText.text = "Cost 100";
        }
        buttonImage.color = buttonColor;
    }

    public void ResetData ( ) {
        // Сбросить данные до значений по умолчанию
        clickSpeed = 0;
        money = 0;

        SaveData ();
        UpdateUI ();
    }

    public void ActivateAutoMining ( ) {
        if ( isButtonPurchased == false ) {
            money = money - 100; // Прямое присваивание, без преобразований
            Debug.Log ("asdl");
            isButtonPurchased = true;
            isPassiveIncomeEnabled = true;
            UpdateUI ();
            UpdateButtonImage ();
        }
    }
}
