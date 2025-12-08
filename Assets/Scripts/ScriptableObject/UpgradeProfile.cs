using UnityEngine;
using System.Collections.Generic;

// 강화 능력치 계산 유형
public enum GrowthType
{
    PerLevel,       // 매 레벨마다 증가 (예: 공격력, 방어력)
    Milestone       // 특정 구간마다 증가 (예: 5강, 10강 때 피해량 증가)
}

[System.Serializable]
public struct StatGrowthRule
{
    public StatType statType;   // 증가 능력치
    public GrowthType type;     // 증가 방식

    [Tooltip("증가량 (티어 1 기준)")]
    public float increaseValue; // 증가 수치

    [Tooltip("강화 규칙")]
    public int startLv;         // 적용 시작 레벨
    public int endLv;           // 적용 종료 레벨
    public int interval;        // 간격 (milestone)
}

[CreateAssetMenu(fileName = "UpgradeProfile", menuName = "Scripts/Data/UpgradeProfile")]
public class UpgradeProfile : ScriptableObject
{
    [Header("강화 규칙 목록")]
    [SerializeField] List<StatGrowthRule> rules;

    public float GetStatIncrease(StatType targetStat, int level)
    {
        float totalIncrease = 0f;

        foreach (var rule in rules)
        {
            // 요청한 스탯에 맞는 규칙 적용
            if (rule.statType != targetStat) continue;

            // 규칙이 적용되는 레벨 구간이 아닌 경우 스킵
            if (level < rule.startLv || level > rule.endLv) continue;

            if (rule.type == GrowthType.PerLevel)
            {
                totalIncrease += rule.increaseValue;
            }
            else if (rule.type == GrowthType.Milestone)
            {
                // 인터벌에 해당하는 레벨일 때
                if (rule.interval > 0 && (level % rule.interval == 0))
                {
                    totalIncrease += rule.increaseValue;
                }
            }
        }

        return totalIncrease;
    }
}
