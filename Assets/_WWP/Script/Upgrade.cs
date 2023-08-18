using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI upgradeInfoText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Clicker clickerScript; // Ссылка на скрипт Clicker
    private UnityEngine.UI.Button button;
    [SerializeField] private long moneyPerSecondIncrease = 0 ; // Изменили тип на ulong
    [SerializeField] private string upgradeID;
    [SerializeField] private int startCost = 1;
    [SerializeField] private int upgradeLevel = 0;
    [SerializeField] private long upgradeCost = 1; // Изменили тип на ulong
    [SerializeField] private long growthMultiplie; // Множитель роста для каждого уровня улучшения
    [SerializeField] private long clickspeed;
    private void Awake ( ) {
        clickspeed = clickerScript.ClickSpeed;
    }
    public void PurchaseUpgrade ( ) {
        if ( clickerScript.Money >= upgradeCost && upgradeLevel < 100 ) {
            upgradeLevel++;
          
            clickerScript.ApplyPassiveIncome (( int ) clickerScript.ClickSpeed); // Приведение обратно к int, так как ClickSpeed типа ulong, а ApplyPassiveIncome ожидает int
            moneyPerSecondIncrease = clickspeed * 2 + growthMultiplie * upgradeLevel;
            moneyPerSecondIncrease /= 7;
            clickerScript.ClickSpeed += moneyPerSecondIncrease; // Не нужно приводить типы, так как оба ulong
            CalculateUpgradeCost ();
            SaveData ();
        

        }
        else if ( upgradeLevel >= 100) {
            costText.text = "MAX";
            upgradeInfoText.text = "MAX";
        }

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

    private void CalculateUpgradeCost ( ) {
        clickerScript.MINUSMoney (upgradeCost);
        upgradeCost = upgradeCost + startCost *  upgradeLevel;
      
        UpdateUpgradeInfo ();
    }

    private void UpdateUpgradeInfo ( ) {
        float moneyInMillions = moneyPerSecondIncrease / 1000000000f;
        upgradeInfoText.text = moneyInMillions.ToString ("F9") +  " " + "B";
        costText.text = GetFormattedMoneyString (( int ) upgradeCost); // Приведение обратно к int для отображения в тексте
    }

    public void StartClass ( ) {
        //ResetData ();
        LoadData (); // Загрузка сохраненных данных
        UpdateUpgradeInfo ();
        button = GetComponent<UnityEngine.UI.Button> ();
        button.onClick.AddListener (PurchaseUpgrade);
    }

    public void ResetData ( ) {
        // Сбросить данные улучшения до исходных значений
        upgradeLevel = 0;
        moneyPerSecondIncrease = 0; // Сбрасываем множитель роста до изначального значения
        SaveData ();
        UpdateUpgradeInfo ();
    }

    private void SaveData ( ) {
        PlayerPrefs.SetInt ($"UpgradeLevel_{upgradeID}", upgradeLevel);
        PlayerPrefs.SetFloat ($"MoneyPerSecondIncrease_{upgradeID}", ( float ) moneyPerSecondIncrease); // Приводим обратно к float для сохранения
        PlayerPrefs.SetFloat ($"UpgradeCost_{upgradeID}", ( float ) upgradeCost); // Приводим обратно к float для сохранения
    }

    private void LoadData ( ) {
        upgradeLevel = PlayerPrefs.GetInt ($"UpgradeLevel_{upgradeID}", 0);
        moneyPerSecondIncrease = ( long ) Mathf.RoundToInt (PlayerPrefs.GetFloat ($"MoneyPerSecondIncrease_{upgradeID}", 1f)); // Приводим обратно к ulong при загрузке
        upgradeCost = ( long ) Mathf.RoundToInt (PlayerPrefs.GetFloat ($"UpgradeCost_{upgradeID}", startCost)); // Приводим обратно к ulong при загрузке
    }
}
