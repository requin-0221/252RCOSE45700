using System;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    // Singleton
    public static PlayerStat Instance { get; private set; }
    
    [Header("최종 전투 스탯")]
    public AttackType FinalAttackType { get; private set; }
    public int FinalMaxHp { get; private set; }
    public int FinalDef { get; private set; }
    public int FinalPAtk { get; private set; }
    public int FinalMAtk { get; private set; }
    public float FinalAtkSpdPercent { get; private set; }
    public float FinalPDmgPercent { get; private set; }
    public float FinalMDmgPercent { get; private set; }
    public float FinalDmgPercent { get; private set; }
    public float FinalCriDmgPercent { get; private set; }
    public float FinalSkillGainPercent { get; private set; }
    public float FinalItemDropPercent { get; private set; }
    public float FinalGoldDropPercent { get; private set; }
    public float FinalCriRate { get; private set; }
    public float FinalDefPntr { get; private set; }
    public float FinalSkillDmg { get; private set; }

    // StatType 열거형의 총 개수
    private readonly int statNum = Enum.GetNames(typeof(StatType)).Length;
    public float[] totalStats;      // 스탯별 총합

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        ResetAllStats(); // initialize
        totalStats = new float[statNum];

        Debug.Log("PlayerStat Initialized.");
    }

    public void CalculateAllStats()
    {
        // Equip Stats
        SumEquipStats();

        if (InventoryManager.Instance.equipments.ContainsKey(EquipSlot.Weapon) &&
            InventoryManager.Instance.equipments[EquipSlot.Weapon] != null)
        {
            FinalAttackType = InventoryManager.Instance.equipments[EquipSlot.Weapon].data.attackType;
        }
        else
        {
            FinalAttackType = AttackType.Physical;
        }
        FinalMaxHp = 5000 + (int)(totalStats[(int)StatType.MaxHp] * (1.0f + totalStats[(int)StatType.MaxHpPercent] * 0.01f));
        FinalDef = (int)(totalStats[(int)StatType.Def] * (1.0f + totalStats[(int)StatType.DefPercent] * 0.01f));
        FinalPAtk = (int)(totalStats[(int)StatType.PAtk] * (1.0f + totalStats[(int)StatType.PAtkPercent] * 0.01f));
        FinalMAtk = (int)(totalStats[(int)StatType.MAtk] * (1.0f + totalStats[(int)StatType.MAtkPercent] * 0.01f));
        FinalAtkSpdPercent = totalStats[(int)StatType.AtkSpdPercent];
        FinalPDmgPercent = totalStats[(int)StatType.PDmgPercent];
        FinalMDmgPercent = totalStats[(int)StatType.MDmgPercent];
        FinalDmgPercent = totalStats[(int)StatType.DmgPercent];
        FinalCriDmgPercent = totalStats[(int)StatType.CriDmgPercent];
        FinalItemDropPercent = totalStats[(int)StatType.ItemDropPercent];
        FinalGoldDropPercent = totalStats[(int)StatType.GoldDropPercent];
        FinalCriRate = StarToCriRate(totalStats[(int)StatType.CriRateStar]);
        FinalDefPntr = StarToDefPntr(totalStats[(int)StatType.CriRateStar]);
        FinalSkillDmg = 300.0f + StarToSkillDmg(totalStats[(int)StatType.CriRateStar]);

        Debug.Log("PlayerStat Calculated.");
        if (UIManager.Instance != null)
            UIManager.Instance.RefreshStatusUI();
    }

    public void SumEquipStats()
    {
        var equipped = InventoryManager.Instance.equipments;
        if (equipped == null) return;

        for (int i = 0; i < statNum; i++) totalStats[i] = 0; // reset

        // 스탯별로 장착된 아이템 전체를 순회하여 합산
        foreach (var item in equipped.Values)
        {
            if (item == null) continue;

            for (int i = 0; i < statNum; i++)
            {
                totalStats[i] += item.totalStats[i];
            }
        }
    }

    public void ResetAllStats()
    {
        // Set default values
        FinalAttackType = AttackType.Physical;
        FinalMaxHp = 5000;
        FinalDef = 0;
        FinalPAtk = 0;
        FinalMAtk = 0;
        FinalAtkSpdPercent = 0f;
        FinalPDmgPercent = 0f;
        FinalMDmgPercent = 0f;
        FinalDmgPercent = 0f;
        FinalCriDmgPercent = 0f;
        FinalItemDropPercent = 0f;
        FinalGoldDropPercent = 0f;
        FinalCriRate = 15f;
        FinalDefPntr = 0f;
        FinalSkillDmg = 300f;
    }

    private float StarToCriRate(float starValue)
    {
        if (starValue <= 0) return 0;
        // 공식: (x + 12) / (x + 120) * 1.5 (Max 1.0)
        float chance = ((starValue + 12f) / (starValue + 120f)) * 1.5f;
        return Mathf.Min(chance, 1.0f) * 100f;
    }

    private float StarToDefPntr(float starValue)
    {
        if (starValue <= 0) return 0;
        // 공식: (x + 12) / (x + 120) * 1.5 (Max 1.0)
        float chance = ((starValue + 12f) / (starValue + 120f)) * 1.5f;
        return Mathf.Min(chance, 1.0f) * 100f;
    }

    private float StarToSkillDmg(float starValue)
    {
        if (starValue <= 0) return 0f;
        // 공식: (x + 12) / (x + 120) * 1.5 (Max 1.0)
        float chance = ((starValue + 12f) / (starValue + 120f)) * 1.5f;
        return Mathf.Min(chance, 1.0f) * 100f;
    }
}
