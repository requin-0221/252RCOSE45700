using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    // Singleton
    public static BattleManager Instance { get; private set; }

    public event Action<long, bool> OnBossHit; // (데미지, 치명타)
    public event Action<int> OnBossAttack; // (데미지)
    // 보스 정보 변경 시 (HP, Shield UI 갱신용)
    public event Action OnBossStatusChanged;
    // 플레이어 정보 변경 시 (HP, SP UI 갱신용)
    public event Action OnPlayerStatusChanged;
    // 전투 종료 이벤트 (승리/패배)
    public event Action<bool> OnBattleEnd; // true: 승리, false: 패배

    [Header("보스 인스턴스")]
    [SerializeField] long bossHp;
    [SerializeField] long bossMaxHp;
    [SerializeField] long bossShield;
    [SerializeField] long bossMaxShield;
    [SerializeField] int bossAtk;
    [SerializeField] int bossDef;
    [SerializeField] float bossAttackDelay;
    [SerializeField] float bossAttackPeriod = 5.0f;

    [Header("플레이어 인스턴스")]
    [SerializeField] float playerSP;
    [SerializeField] float playerMaxSP = 15.0f;
    public int playerHP { get; private set; }
    public int playerMaxHP { get; private set; }
    [SerializeField] float playerAttackDelay;
    [SerializeField] float playerAttackPeriod;

    private bool isBattleActive = true; // 전투 지속 여부

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!isBattleActive) return;

        ProcessTimers();
    }

    void Initialize()
    {
        // BossManager와 PlayerStat 참조 필수
        if (BossManager.Instance == null || PlayerStat.Instance == null) { return; }

        // 보스 정보 초기화
        BossData data = BossManager.Instance.GetCurrentBossData();
        if (data == null) { return; }

        BossDifficulty difficulty = BossManager.Instance.difficulty;
        if (!data.SpecDict.ContainsKey(difficulty)) return;

        bossHp = bossMaxHp = data.SpecDict[difficulty].maxHp;
        bossShield = bossMaxShield = data.SpecDict[difficulty].maxShield;
        bossAtk = data.SpecDict[difficulty].atk;
        bossDef = data.SpecDict[difficulty].def;
        bossAttackDelay = 0f;

        // 플레이어 정보 초기화
        playerHP = playerMaxHP = PlayerStat.Instance.FinalMaxHp;
        playerSP = 0f;
        playerAttackDelay = 0f;
        playerAttackPeriod = 1f / (1f + PlayerStat.Instance.FinalAtkSpdPercent * 0.01f);

        OnBossStatusChanged?.Invoke();
        OnPlayerStatusChanged?.Invoke();
    }

    void ProcessTimers()
    {
        float dt = Time.deltaTime;

        // 플레이어 공격 타이머
        playerAttackDelay += dt;
        if (playerAttackDelay >= playerAttackPeriod)
        {
            playerAttackDelay -= playerAttackPeriod;
            PerformPlayerAttack();
        }

        // 보스 공격 타이머
        bossAttackDelay += dt;
        if (bossAttackDelay >= bossAttackPeriod)
        {
            bossAttackDelay -= bossAttackPeriod;
            PerformBossAttack();
        }
    }

    float GetFinalNormalDmg()
    {
        if (PlayerStat.Instance == null) return 1;

        int playerAtk = PlayerStat.Instance.GetFinalAtk();
        
        bool isDefPntr = UnityEngine.Random.value < (PlayerStat.Instance.FinalDefPntr * 0.01f); // 방어 관통 판정

        int bossRealDef = bossDef;
        if (bossShield <= 0 || isDefPntr) bossRealDef = 0;

        float a = Mathf.Min(0, playerAtk - bossRealDef); // 보스 방어력 차감 보정

        float dmg = Mathf.Pow((float)(playerAtk - bossRealDef), 1.5f);
        dmg = dmg * (1f + (PlayerStat.Instance.GetFinalBonusDmgPercent()) * 0.01f);

        return dmg;
    }

    // 플레이어가 보스를 때림 (BossHit)
    void PerformPlayerAttack()
    {
        if (PlayerStat.Instance == null) { return; }

        // 데미지 계산
        float dmg = GetFinalNormalDmg();

        bool isCrit = UnityEngine.Random.value < (PlayerStat.Instance.FinalCriRate * 0.01f); // 치명타 판정
        if (isCrit) dmg = dmg * (1f + PlayerStat.Instance.FinalCriDmgPercent * 0.01f);

        bool isSkill = (playerSP >= playerMaxSP); // 스킬 사용 여부
        if (isSkill)
        {
            dmg = dmg * PlayerStat.Instance.FinalSkillDmg * 0.01f;
            playerSP = 0f;
        }
        else
        {
            // SP 획득 로직 (타격당)
            playerSP = Mathf.Min(playerSP + (1 + PlayerStat.Instance.FinalSkillGainPercent * 0.01f), playerMaxSP);
        }

        // 무작위 편차
        float d = UnityEngine.Random.Range(0.9f, 1.1f);
        dmg = Mathf.Floor(dmg * d);

        if (dmg < 1f) dmg = 1f; // 최소 데미지 보정
        long damage = (long)dmg;

        // 보스에게 데미지 적용
        ApplyDamageToBoss(damage);

        // 이벤트 발생 (UI, 이펙트 등에서 수신)
        OnBossHit?.Invoke(damage, isCrit);
        OnPlayerStatusChanged?.Invoke();
        OnBossStatusChanged?.Invoke();

        Debug.Log($"플레이어 공격 : 데미지 {damage}");

        CheckBattleEnd();
    }

    // 보스가 플레이어를 때림 (BossAttack)
    void PerformBossAttack()
    {
        if (PlayerStat.Instance == null) { return; }

        long damage = bossAtk - PlayerStat.Instance.FinalDef;
        // 무작위 편차
        float d = UnityEngine.Random.Range(0.9f, 1.1f);
        damage = (long)Mathf.Floor(damage * d);

        if (damage < 1) damage = 1; // 최소 데미지 보정

        // 플레이어에게 데미지 적용
        playerHP -= (int)damage;
        if (playerHP < 0) playerHP = 0;

        // 이벤트 발생
        OnBossAttack?.Invoke((int)damage);
        OnPlayerStatusChanged?.Invoke();

        Debug.Log($"보스 공격 : 데미지 {damage}");

        CheckBattleEnd();
    }

    // 보스 데미지 적용
    void ApplyDamageToBoss(long damage)
    {
        // 쉴드 차감
        if (bossShield > 0)
        {
            if (bossShield >= damage)
            {
                bossShield -= damage;
            }
            else
            {
                bossShield = 0;
            }
        }

        // HP 차감
        bossHp -= damage;
        if (bossHp < 0) bossHp = 0;
    }

    // 전투 종료 판정 검사
    void CheckBattleEnd()
    {
        if (isBattleActive == false) return; 
        if (bossHp <= 0)
        {
            isBattleActive = false;
            OnBattleEnd?.Invoke(true); // 승리
            Debug.Log("Victory!");
        }
        else if (playerHP <= 0)
        {
            isBattleActive = false;
            OnBattleEnd?.Invoke(false); // 패배
            Debug.Log("Defeat.");
        }
    }

    public float GetHpRatio()
    {
        return bossMaxHp == 0 ? 0 : (float)bossHp / bossMaxHp;
    }

    public float GetShieldRatio()
    {
        return bossMaxShield == 0 ? 0 : (float)bossShield / bossMaxShield;
    }

    public float GetPlayerHpRatio()
    {
        return playerMaxHP == 0 ? 0 : (float)playerHP / playerMaxHP;
    }

    public float GetSkillGaugeRatio()
    {
        return playerMaxSP == 0 ? 0 : playerSP / playerMaxSP;
    }
}
