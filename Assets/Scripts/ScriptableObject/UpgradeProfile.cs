using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct StatIncreaseList
{
    public StatType statType;   // 증가 능력치
    [Tooltip("증가 수치 리스트")]
    public float[] values;
}

[CreateAssetMenu(fileName = "UpgradeProfile", menuName = "Scripts/Data/UpgradeProfile")]
public class UpgradeProfile : ScriptableObject
{
    [Header("강화 규칙 목록")]
    public List<StatIncreaseList> rules;

    public float GetStatIncrease(StatType targetStat, int level)
    {
        foreach (var rule in rules)
        {
            // 요청한 스탯에 맞는 규칙 적용
            if (rule.statType != targetStat) continue;
            return rule.values[level];
        }
        return 0f;
    }
}
