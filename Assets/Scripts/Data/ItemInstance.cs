using System;
using System.Collections.Generic;

[Serializable]
public class ItemInstance
{
    public ItemData data;

    // 고유 ID
    public string uniqueID;
    
    // 상태
    public int upgradeLv; // 일반 강화 레벨
    public int growthLv; // 성장 레벨(총 스택)

    // 성장 능력치 스택 (Key: 능력치, Value: 스택 수)
    public Dictionary<StatType, int> growthStacks;

    // 능력치
    private readonly int statArraySize; // StatType 열거형의 총 개수

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
        statArraySize = Enum.GetNames(typeof(StatType)).Length;

        baseStats = new float[statArraySize];
        upgradeStats = new float[statArraySize];
        growthStats = new float[statArraySize];
        totalStats = new float[statArraySize];

        calculateStats();
    }

    public void calculateStats()
    {
        // 모든 StatType을 0부터 끝까지 순회
        for (int i = 0; i < statArraySize; i++)
        {
            StatType type = (StatType)i; // 정수를 다시 Enum 타입으로 변환

            // 2. 각 배열의 값을 채움
            baseStats[i] = data.GetBaseStatValue(type);
            upgradeStats[i] = 100;
            growthStats[i] = 100;

            // 3. 최종 총합 계산
            totalStats[i] = baseStats[i] + upgradeStats[i] + growthStats[i];
        }
    }
}