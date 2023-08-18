using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletManager : Singleton<WalletManager>
{
    public int MoneyAmount { get; private set; }

    private int _limit = 999;

    public void Restart()
    {
        MoneyAmount = 20;
    }

    public void IncreaseMoneyAmout(int value)
    {
        MoneyAmount += value;
        if (MoneyAmount > _limit)
            MoneyAmount = _limit;
    }

    public bool CheckMoney(int price)
    {
        return MoneyAmount >= price;
    }

    public void Enhance(int price)
    {
        MoneyAmount -= price;
        if (MoneyAmount < 0)
            MoneyAmount = 0;
    }
}
