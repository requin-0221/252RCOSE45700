using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatData
{
    public StatType statType;
    public float value;
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]

    [Tooltip("아이템 이름")]
    public string itemName;

    [Tooltip("아이템 아이콘")]
    public Sprite icon;

    [Tooltip("아이템 기본 가격")]
    public long baseSellPrice = 0;

    [Header("장비 분류")]
    [Tooltip("장착 부위")]
    public EquipSlot equipSlot = EquipSlot.Weapon;

    [Tooltip("장비 티어 (1 - 20)")]
    [Range(1, 20)]
    public int itemTier = 1;

    [Tooltip("최대 강화 수치")]
    public int maxUpgrade = 10;

    [Tooltip("최대 성장 수치")]
    public int maxGrowth = 30;


    [Header("무기 전용 설정")]
    [Tooltip("공격 유형")]
    public AttackType attackType = AttackType.Physical;


    [Header("장비 기본 스탯")]
    public List<StatData> baseStats;

    public float GetBaseStatValue(StatType type)
    {
        foreach (var stat in baseStats)
        {
            if (stat.statType == type)
                return stat.value;
        }
        return 0f;
    }

    [Header("강화 설정")]
    [Tooltip("아이템 강화 / 성장 규칙")]
    public UpgradeProfile upgradeProfile;
    public GrowthProfile growthProfile;
}