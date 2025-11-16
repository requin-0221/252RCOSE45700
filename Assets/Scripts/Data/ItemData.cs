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
    None,
    MaxHp,         // 체력 (단순)
    Def,           // 방어력 (단순)
    PAtk,         // 물리 공격력 (단순)
    MAtk,         // 마법 공격력 (단순)

    // % 수치 (Percent)
    MaxHpPercent,       // 체력 %
    DefPercent,         // 방어력 %
    PAtkPercent,       // 물리 공격력 %
    MAtkPercent,       // 마법 공격력 %
    PDmgPercent,       // 물리 피해량 %
    MDmgPercent,       // 마법 피해량 %
    DmgPercent,         // 추가 피해량 %
    AtkSpdPercent,      // 공격 속도 %
    SkillGainPercent,   // 스킬 자원 획득 %
    CriDmgPercent,      // 치명타 피해량 %
    ItemDropPercent,    // 아이템 획득 확률 % (장신구)
    GoldDropPercent,    // 재화 획득 확률 % (장신구)

    // ★ 수치 (Star)
    DefPntrStar,   // 방어 관통 ★
    CriRateStar,         // 치명타 확률 ★
    SkillDmgStar          // 스킬 강화 ★
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scripts/Data/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("--- 기본 정보 ---")]

    [Tooltip("아이템 이름")]
    public string itemName;

    [Tooltip("아이템 설명")]
    [TextArea(3, 10)]
    public string description;

    [Tooltip("아이템 아이콘")]
    public Sprite icon;


    [Header("--- 장비 분류 ---")]
    [Tooltip("장착 부위")]
    public EquipSlot equipSlot = EquipSlot.Weapon;

    [Tooltip("장비 티어 (1 - 20)")]
    [Range(1, 20)]
    public int itemTier = 1;

    [Tooltip("최대 강화 수치")]
    public int maxUpgrade = 10;

    [Tooltip("최대 성장 수치")]
    public int maxGrowing = 40;


    [Header("--- 무기 전용 설정 (Weapon 슬롯만 적용) ---")]
    [Tooltip("공격 유형")]
    public AttackType attackType = AttackType.Physical;


    [Header("--- 장비 기본 스탯 ---")]
    public int baseMaxHp;           // 기본 체력
    public int baseDef;             // 기본 방어력
    public int basePAtk;            // 기본 물리 공격력
    public int baseMAtk;            // 기본 마법 공격력


    [Header("--- 퍼센트 스탯 (0.1 = 10%) ---")]
    [Tooltip("공격 관련")]
    [Range(0f, 1f)] public float basePAtkPercent;   // 물리 공격력 %
    [Range(0f, 1f)] public float baseMAtkPercent;   // 마법 공격력 %
    [Range(0f, 1f)] public float basePDmgPercent;   // 물리 피해량 %
    [Range(0f, 1f)] public float baseMDmgPercent;   // 마법 피해량 %
    [Range(0f, 1f)] public float baseDmgPercent;    // 추가 피해량 %
    [Range(0f, 1f)] public float baseCriDmgPercent; // 치명타 피해량 %
    [Range(0f, 1f)] public float baseAtkSpdPercent; // 공격 속도 %

    [Tooltip("방어/유틸 관련")]
    [Range(0f, 1f)] public float baseDefPercent;        // 방어력 %
    [Range(0f, 1f)] public float baseMaxHpPercent;      // 체력 %
    [Range(0f, 1f)] public float baseSkillGainPercent;  // 스킬 자원 획득 % (모자)

    [Tooltip("파밍 관련")]
    [Range(0f, 1f)] public float baseItemDropPercent;   // 아이템 획득 확률 %
    [Range(0f, 1f)] public float baseGoldDropPercent;   // 재화 획득 확률 %


    [Header("--- ★ 수치 (Star) ---")]
    [Tooltip("치명타 확률 ★")]
    public float baseCriRateStar;
    [Tooltip("방어 관통 ★")]
    public float baseDefPntrStar;
    [Tooltip("스킬 강화 ★")]
    public float baseSkillDmgStar;
}