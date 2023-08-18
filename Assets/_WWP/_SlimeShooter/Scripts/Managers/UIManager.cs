using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI _wallet;

    [SerializeField] private TextMeshProUGUI _attackRatePrice;
    [SerializeField] private TextMeshProUGUI _attackRate;

    [SerializeField] private TextMeshProUGUI _damagePrice;
    [SerializeField] private TextMeshProUGUI _damage;

    [SerializeField] private TextMeshProUGUI _healthPrice;

    public void Restart()
    {
        UIUpdateData data = new UIUpdateData();
        data.playerDamage = SlimeShooterManager.Instance.playerConfig.damage.value;
        data.playerAttackRate = SlimeShooterManager.Instance.playerConfig.attackRate.value;
        data.attackRateEnhancementPrice = EnhancementManager.Instance.AttackRatePrice;
        data.damageEnhancementPrice = EnhancementManager.Instance.DamagePrice;
        data.healthRecoveryPrice = EnhancementManager.Instance.HealthPrice;

        UpdateUI(data);
    }

    public void Enhance(string enhancementName)
    {
        if (!EnhancementManager.Instance.Enhance(enhancementName))
        {
            if (enhancementName == "Damage")
            {
                _damagePrice.text = "MAX";
            }
            if (enhancementName == "AttackRate")
            {
                _attackRatePrice.text = "MAX";
            }
        }
    }

    public void UpdateUI(UIUpdateData data)
    {
        data.attackRateEnhancementPrice = EnhancementManager.Instance.AttackRatePrice;
        data.damageEnhancementPrice = EnhancementManager.Instance.DamagePrice;

        _wallet.text = WalletManager.Instance.MoneyAmount.ToString();

        _attackRatePrice.text = data.attackRateEnhancementPrice.ToString();
        float attacksPerSecond = 1 / data.playerAttackRate;
        _attackRate.text = attacksPerSecond.ToString("#0.##");
        if (data.arMax)
        {
            _attackRatePrice.text = "MAX";
        }

        _damagePrice.text = data.damageEnhancementPrice.ToString();
        _damage.text = data.playerDamage.ToString();
        if (data.damageMax)
        {
            _damagePrice.text = "MAX";
        }

        _healthPrice.text = data.healthRecoveryPrice.ToString();
    }

    public void UpdateUI()
    {
        _wallet.text = WalletManager.Instance.MoneyAmount.ToString();
    }
}

public struct UIUpdateData
{
    public float playerDamage;
    public bool damageMax;
    public float playerAttackRate;
    public bool arMax;
    public int attackRateEnhancementPrice;
    public int damageEnhancementPrice;
    public int healthRecoveryPrice;
}
