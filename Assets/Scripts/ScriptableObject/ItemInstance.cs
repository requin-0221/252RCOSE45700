using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemInstance
{
    public ItemData data;

    // 고유 ID
    public string uniqueID;
    
    // 상태
    public int upgradeLv;   // 일반 강화 레벨
    public int growthLv;    // 성장 레벨(총 스택)
    public readonly int maxGrowthStackLv = 15; // 스택 당 최대 성장 단계

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
            //growthStats[i] = 10;
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

    public int GetGrowthCost()
    {
        return (int)Mathf.Floor((growthLv + 1) * 100 * Mathf.Sqrt(data.itemTier));
    }

    public long GetGrowthStackRestoreCost(int stackNum)
    {
        if (stackNum <= 0) return 0;

        stackNum = Mathf.Clamp(stackNum, 1, 15);
        return (long)Mathf.Floor(100f * Mathf.Pow(data.itemTier, 1.5f) * Mathf.Pow((growthLv-stackNum+1), 2f));
    }

    public long GetEntireRestoreCost()
    {
        if (growthLv <= 0) return 0;
        return (long)Mathf.Floor(100f * Mathf.Pow(data.itemTier, 1.5f) * growthLv * growthLv);
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

    public void Growing()
    {
        if (growthLv >= data.maxGrowth) return;

        // 무작위로 성장할 스탯 결정
        int statCount = data.growthProfile.rules.Count;
        int i;        
        StatType type;

        while (true) 
        {
            i = UnityEngine.Random.Range(0, statCount);
            type = data.growthProfile.rules[i].statType;

            // 아직 스택이 없는 능력치인지 검사
            if (growthStacks.ContainsKey(type) == false)
            {
                growthStacks.Add(type, 1);
                Debug.Log($"성장 스택 새로 추가됨 : {type.ToString()}");
                break;
            }

            if (growthStacks[type] < maxGrowthStackLv)
            {
                growthStacks[type]++;
                break;
            }
        }

        float value = data.growthProfile.rules[i].values[growthStacks[type]];

        if (type == StatType.MaxHp || type == StatType.Def ||
            type == StatType.PAtk || type == StatType.MAtk)
        {
            value *= (1 + data.itemTier / 6);
        }
        else
        {
            value += data.itemTier / 6;
        }

        growthStats[(int)type] += value;

        growthLv++;
        calculateStats();
    }

    public void GrowthStackRestore(StatType type)
    {
        if (!growthStacks.ContainsKey(type)) return;

        growthStats[(int)type] = 0;
        growthLv -= growthStacks[type];
        growthStacks.Remove(type);
        calculateStats();
    }

    public void GrowthEntireRestore()
    {
        growthLv = 0;
        for (int i = 0; i < growthStats.Length; i++)
        {
            growthStats[i] = 0;
        }
        
        growthStacks.Clear();
        calculateStats();
    }
}