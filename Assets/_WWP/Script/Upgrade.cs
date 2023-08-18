using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI upgradeInfoText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Clicker clickerScript; // ������ �� ������ Clicker
    private UnityEngine.UI.Button button;
    [SerializeField] private long moneyPerSecondIncrease = 0 ; // �������� ��� �� ulong
    [SerializeField] private string upgradeID;
    [SerializeField] private int startCost = 1;
    [SerializeField] private int upgradeLevel = 0;
    [SerializeField] private long upgradeCost = 1; // �������� ��� �� ulong
    [SerializeField] private long growthMultiplie; // ��������� ����� ��� ������� ������ ���������
    [SerializeField] private long clickspeed;
    private void Awake ( ) {
        clickspeed = clickerScript.ClickSpeed;
    }
    public void PurchaseUpgrade ( ) {
        if ( clickerScript.Money >= upgradeCost && upgradeLevel < 100 ) {
            upgradeLevel++;
          
            clickerScript.ApplyPassiveIncome (( int ) clickerScript.ClickSpeed); // ���������� ������� � int, ��� ��� ClickSpeed ���� ulong, � ApplyPassiveIncome ������� int
            moneyPerSecondIncrease = clickspeed * 2 + growthMultiplie * upgradeLevel;
            moneyPerSecondIncrease /= 7;
            clickerScript.ClickSpeed += moneyPerSecondIncrease; // �� ����� ��������� ����, ��� ��� ��� ulong
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
        costText.text = GetFormattedMoneyString (( int ) upgradeCost); // ���������� ������� � int ��� ����������� � ������
    }

    public void StartClass ( ) {
        //ResetData ();
        LoadData (); // �������� ����������� ������
        UpdateUpgradeInfo ();
        button = GetComponent<UnityEngine.UI.Button> ();
        button.onClick.AddListener (PurchaseUpgrade);
    }

    public void ResetData ( ) {
        // �������� ������ ��������� �� �������� ��������
        upgradeLevel = 0;
        moneyPerSecondIncrease = 0; // ���������� ��������� ����� �� ������������ ��������
        SaveData ();
        UpdateUpgradeInfo ();
    }

    private void SaveData ( ) {
        PlayerPrefs.SetInt ($"UpgradeLevel_{upgradeID}", upgradeLevel);
        PlayerPrefs.SetFloat ($"MoneyPerSecondIncrease_{upgradeID}", ( float ) moneyPerSecondIncrease); // �������� ������� � float ��� ����������
        PlayerPrefs.SetFloat ($"UpgradeCost_{upgradeID}", ( float ) upgradeCost); // �������� ������� � float ��� ����������
    }

    private void LoadData ( ) {
        upgradeLevel = PlayerPrefs.GetInt ($"UpgradeLevel_{upgradeID}", 0);
        moneyPerSecondIncrease = ( long ) Mathf.RoundToInt (PlayerPrefs.GetFloat ($"MoneyPerSecondIncrease_{upgradeID}", 1f)); // �������� ������� � ulong ��� ��������
        upgradeCost = ( long ) Mathf.RoundToInt (PlayerPrefs.GetFloat ($"UpgradeCost_{upgradeID}", startCost)); // �������� ������� � ulong ��� ��������
    }
}
