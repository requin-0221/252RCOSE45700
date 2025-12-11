using UnityEngine;
using System.Collections.Generic;

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


public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance {get; private set;}

    [Header("Singletons")]
    [SerializeField] List<GameObject> singletonList;

    [Header("Resources")]
    [SerializeField] private long _gold = 0;
    public long gold {
        get 
        {
            return _gold; 
        } 
        private set 
        {
            _gold = value;
            GoldChanged(); 
        } 
    }
    const long maxGold = 999999999999;

    [SerializeField] private int _stone = 0;
    public int stone
    {
        get
        {
            return _stone;
        }
        private set
        {
            _stone = value;
            StoneChanged();
        }
    }
    const int maxStone = 99999999;

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
        // 기타 싱글톤 생성
        foreach (var go in singletonList)
        {
            Instantiate(go);
        }

        Debug.Log("GameManager Initialized.");
    }

    public void AddGold(long value)
    {
        gold += value;
    }

    public void AddStone(int value)
    {
        stone += value;
    }

    void GoldChanged()
    {
        if (gold > maxGold)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPopup(
                    $"골드 보유량 제한을 달성하였습니다.\n{(_gold - maxGold).ToString("N0")} 골드가 사라집니다."
                );
            }
            _gold = maxGold;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshResourceUI();
        }
    }

    void StoneChanged()
    {
        if (stone > maxStone)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPopup(
                    $"마석 보유량 제한을 달성하였습니다.\n{(_stone - maxStone).ToString("N0")}개의 마석이 사라집니다."
                );
            }
            _stone = maxStone;
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshResourceUI();
        }
    }
}