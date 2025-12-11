using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GrowthProfile", menuName = "Scriptable Objects/GrowthProfile")]
public class GrowthProfile : ScriptableObject
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
