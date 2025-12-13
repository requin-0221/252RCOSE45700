using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    // Singleton
    public static BattleUIManager Instance { get; private set; }

    [Header("UI components")]
    [SerializeField] BossGaugeUI bossGaugeUI;
    [SerializeField] BossIllust bossIllust;
    [SerializeField] RectTransform bossDmgSpawnpoint;
    [SerializeField] RectTransform playerDmgSpawnpoint;
    [SerializeField] PlayerGaugeUI playerGaugeUI;
    [SerializeField] Image fogImage;
    [SerializeField] List<Color> _fogColors;
    public IReadOnlyList<Color> fogColors => _fogColors;

    [Header("Button")]
    [SerializeField] Button pauseButton;

    [Header("Prefab")]
    public GameObject messageGroupPrefab; // 인스펙터에서 할당
    public Transform popupParent;         // 팝업이 생성될 Canvas
    public DmgTextUI bossDmgTextNormal;
    public DmgTextUI bossDmgTextCritical;
    public DmgTextUI playerDmgText;

    [Header("Scene Transition")]
    public SceneTransition sceneTransition;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (bossGaugeUI != null) bossGaugeUI.UpdateUI();
        if (playerGaugeUI != null) playerGaugeUI.UpdateUI();

        if (fogImage != null)
        {
            if (BossManager.Instance != null) 
            {
                fogImage.color = fogColors[(int)BossManager.Instance.difficulty];
            }
        }

        if (pauseButton != null) pauseButton.onClick.AddListener(OnClickPause);

        if (BattleManager.Instance != null)
        {
            // 이벤트 추가
            BattleManager.Instance.OnBossStatusChanged += (() => bossGaugeUI.UpdateUI());
            BattleManager.Instance.OnBossHit += OnBossHit;
            BattleManager.Instance.OnBossAttack += OnBossAttack;
            BattleManager.Instance.OnPlayerStatusChanged += (() => playerGaugeUI.UpdateUI());
            BattleManager.Instance.OnBattleEnd += OnBattleEnd;
        }
    }

    private void Update()
    {
        
    }

    public void RefreshAllUI()
    {
        if (bossGaugeUI) bossGaugeUI.UpdateUI();
        if (playerGaugeUI) playerGaugeUI.UpdateUI();
    }

    public void ShowPopup(string message, Action btn_callback = null, Action close_callback = null, string btnTxt = "확인", string title = "Message")
    {
        GameObject obj = Instantiate(messageGroupPrefab, popupParent);
        MessageUI popup = obj.GetComponent<MessageUI>();
        popup.Setup(message, btn_callback, close_callback, btnTxt, title);
    }

    public void OnClickPause()
    {
        // 일시정지
        Time.timeScale = 0f;
        ShowPopup("전투를 중단하시겠습니까?", Retire, () => { Time.timeScale = 1f; });
    }

    public void Retire()
    {
        sceneTransition.StartFadeOut("MainScene");
        Time.timeScale = 1f;
    }

    public void OnBossHit(long dmgValue, bool isCrit)
    {
        // 보스 피격 이펙트
        if (bossIllust) bossIllust.OnBossHit();

        // 데미지 출력
        if (bossDmgSpawnpoint)
        {
            if (isCrit)
            {
                DmgTextUI obj = Instantiate(bossDmgTextCritical, bossDmgSpawnpoint);
                obj.SetText(dmgValue.ToString("N0"));
                Vector2 dir = new Vector2(UnityEngine.Random.Range(-0.2f, 0.2f), 1f);
                obj.SetMove(dir, 200f, 0.25f);
            }
            else
            {
                DmgTextUI obj = Instantiate(bossDmgTextNormal, bossDmgSpawnpoint);
                obj.SetText(dmgValue.ToString("N0"));
                Vector2 dir = new Vector2(UnityEngine.Random.Range(-0.2f, 0.2f), 1f);
                obj.SetMove(dir, 200f, 0.25f);
            }
        }
    }

    public void OnBossAttack(int dmgValue)
    {
        // 보스 공격 이펙트
        bossIllust.OnBossAttack();

        // 피격 데미지 출력
        if (playerDmgSpawnpoint)
        {
            DmgTextUI obj = Instantiate(playerDmgText, playerDmgSpawnpoint);
            obj.SetText("-" + dmgValue.ToString("N0"));
            Vector2 dir = new Vector2(UnityEngine.Random.Range(-0.2f, 0.2f), 1f);
            obj.SetMove(dir, 100f, 0.25f);
        }
    }

    public void OnBattleEnd(bool isClear)
    {
        if (isClear)
        {
            Time.timeScale = 0f;
            bossIllust.gameObject.SetActive(false);

            ShowPopup("보스를 격파했습니다.", OnClear, OnClear);
        }
        else
        {
            Time.timeScale = 0f;
            ShowPopup("사망하였습니다.\n로비로 돌아갑니다.", Retire, Retire);
        }
    }

    public void OnClear()
    {
        if (BossManager.Instance == null)
        {
            ShowPopup("오류로 보상이 지급되지 않습니다.", Retire, Retire);
            return;
        }

        BossManager.Instance.AddReward();

        ShowPopup("보상이 지급되었습니다.", Retire, Retire);
    }
}