using System.Collections.Generic;
using UnityEngine;

public enum EquipSlot // 장비 슬롯
{
    None,           // 장착 부위 없음
    // 무기류
    Weapon,         // 무기
    Subweapon,      // 보조무기
    // 방어구
    Head,           // 모자
    Top,            // 상의
    Bottom,         // 하의
    Shoes,          // 신발
    Gloves,         // 장갑
    Cape,           // 망토
    // 장신구
    Bracelet,       // 팔찌
    Ring,           // 반지
    Neckless,       // 목걸이
    Earrings,       // 귀고리
    Face            // 얼굴장식
}

public enum AttackType // 무기에만 적용
{
    Physical,   // 물리
    Magical     // 마법
}

// 능력치 종류
public enum StatType
{
    AtkSpdPercent,      // 공격 속도 %

    MaxHp,              // 체력 (단순)
    Def,                // 방어력 (단순)
    PAtk,               // 물리 공격력 (단순)
    MAtk,               // 마법 공격력 (단순)

    PDmgPercent,        // 물리 피해량 %
    MDmgPercent,        // 마법 피해량 %
    DmgPercent,         // 추가 피해량 %

    CriRateStar,        // 치명타 확률 ★
    DefPntrStar,        // 방어 관통 ★
    SkillDmgStar,        // 스킬 강화 ★

    MaxHpPercent,       // 체력 %
    DefPercent,         // 방어력 %
    PAtkPercent,        // 물리 공격력 %
    MAtkPercent,        // 마법 공격력 %
    
    CriDmgPercent,      // 치명타 피해량 %
    SkillGainPercent,   // 스킬 자원 획득 %
    ItemDropPercent,    // 아이템 획득 확률 % (장신구)
    GoldDropPercent,    // 재화 획득 확률 % (장신구)
}

[System.Serializable]
public struct StatData
{
    public StatType statType;
    public float value;
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scripts/Data/ItemData")]
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
    public int maxGrowth = 40;


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
}