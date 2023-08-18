using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnhancementManager : Singleton<EnhancementManager>
{
    public int AttackRatePrice { get; private set; }
    public int DamagePrice { get; private set; }
    public int HealthPrice { get; private set; }

    public void Restart()
    {
        AttackRatePrice = SlimeShooterManager.Instance.enhancementData.attackRateStartPrice;
        DamagePrice = SlimeShooterManager.Instance.enhancementData.damageStartPrice;
        HealthPrice = SlimeShooterManager.Instance.enhancementData.healthRecoveryPrice;
    }

    public bool Enhance(string enhancementName)
    {
        Enhancement enhancement = new Enhancement();
        enhancement.targetParameterName = enhancementName;
        if (SlimeShooterManager.Instance.Player.CheckParameterIsMax(enhancement.targetParameterName))
        {
            return false;
        }
        switch (enhancementName)
        {
            case "AttackRate":
                enhancement.price = AttackRatePrice;
                enhancement.value = SlimeShooterManager.Instance.enhancementData.attackRateUpgrade;
                if (WalletManager.Instance.CheckMoney(enhancement.price))
                {
                    AttackRatePrice += SlimeShooterManager.Instance.enhancementData.attackRatePriceFactor;
                    SlimeShooterManager.Instance.Enhance(enhancement);
                }
                break;
            case "Damage":
                enhancement.price = DamagePrice;
                enhancement.value = SlimeShooterManager.Instance.enhancementData.damageUpgrade;
                if (WalletManager.Instance.CheckMoney(enhancement.price))
                {
                    DamagePrice += SlimeShooterManager.Instance.enhancementData.damagePriceFactor;
                    SlimeShooterManager.Instance.Enhance(enhancement);
                }
                break;
            case "Health":
                enhancement.price = HealthPrice;
                enhancement.value = SlimeShooterManager.Instance.enhancementData.healthRecovery;
                SlimeShooterManager.Instance.Enhance(enhancement);
                break;
        }
        return true;
    }
}

[Serializable]
public class EnhancementData
{
    public int attackRateStartPrice;
    public int attackRatePriceFactor;

    public int damageStartPrice;
    public int damagePriceFactor;

    public int healthRecoveryPrice;

    public float attackRateUpgrade;
    public float damageUpgrade;
    public float healthRecovery;
}
