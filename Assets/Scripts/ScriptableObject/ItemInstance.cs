using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using static UnityEditor.Progress;

[Serializable]
public class ItemInstance
{
    public ItemData data;

    // 고유 ID
    public string uniqueID;
    
    // 상태
    public int upgradeLv;   // 일반 강화 레벨
    public int growthLv;    // 성장 레벨(총 스택)

    // 성장 능력치 스택 (Key: 능력치, Value: 스택 수)
    public Dictionary<StatType, int> growthStacks;

    // 능력치
    private readonly int statNum;   // StatType 열거형의 총 개수
    public float[] baseStats;       // 기본 스탯 (ItemData)
    public float[] upgradeStats;    // 강화 증가치
    public float[] growthStats;     // 성장 증가치
    public float[] totalStats;      // 총합 (base + upgrade + growth)

    // 생성자 (아이템 획득 시 호출)
    public ItemInstance(ItemData itemData)
    {
        this.data = itemData;
        this.uniqueID = Guid.NewGuid().ToString(); // 고유값 생성
        this.upgradeLv = 0;
        this.growthLv = 0;
        this.growthStacks = new Dictionary<StatType, int>();
        statNum = Enum.GetNames(typeof(StatType)).Length;

        baseStats = new float[statNum];
        upgradeStats = new float[statNum];
        growthStats = new float[statNum];
        totalStats = new float[statNum];

        calculateStats();
    }

    public void calculateStats()
    {
        for (int i = 0; i < statNum; i++)
        {
            StatType type = (StatType)i; // 정수를 다시 Enum 타입으로 변환

            baseStats[i] = data.GetBaseStatValue(type);
            //upgradeStats[i] = 10;
            growthStats[i] = 10;
            totalStats[i] = baseStats[i] + upgradeStats[i] + growthStats[i];
        }
    }

    public long GetUpgradeCumulativeCost()
    {
        return (long)Mathf.Floor(UpgradeConfig.Instance.GetCumulativeCost(upgradeLv) * Mathf.Sqrt(data.itemTier));
    }

    public long GetUpgradeCost()
    {
        return (long)Mathf.Floor(UpgradeConfig.Instance.GetUpgradeCost(upgradeLv) * Mathf.Sqrt(data.itemTier));
    }

    public long GetSellPrice()
    {
        return data.baseSellPrice + GetUpgradeCumulativeCost() + (growthLv * 100);
    }

    public void Upgrade()
    {
        foreach (var rule in data.upgradeProfile.rules)
        {
            int i = (int)rule.statType;
            float value = rule.values[upgradeLv];

            if (value == 0) continue;

            if (i == (int)StatType.MaxHp || i == (int)StatType.Def ||
                i == (int)StatType.PAtk || i == (int)StatType.MAtk)
            {
                value *= Mathf.Floor(Mathf.Sqrt(data.itemTier));
            }
            upgradeStats[i] += value;
        }

        upgradeLv++;
        calculateStats();
    }
}