using TMPro;
using UnityEngine;

public class RewardInfoUI : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField] CanvasGroup upgradeGroup;

    [Header("UI Component")]
    [SerializeField] TextMeshProUGUI firstGoldText;
    [SerializeField] TextMeshProUGUI firstStoneText;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] TextMeshProUGUI stoneText;
    [SerializeField] TextMeshProUGUI minTierText;
    [SerializeField] TextMeshProUGUI maxTierText;
    [SerializeField] TextMeshProUGUI countRateText;

    private void Awake()
    {
        if (upgradeGroup == null) upgradeGroup = GetComponent<CanvasGroup>();
    }

    public void UpdateUI()
    {
        if (BossManager.Instance == null || UIManager.Instance == null) return;

        BossData data = BossManager.Instance.GetCurrentBossData();
        if (data == null) return;

        BossDifficulty difficulty = BossManager.Instance.difficulty;
        if (!data.SpecDict.ContainsKey(difficulty)) return;

        long goldValue = data.SpecDict[difficulty].goldReward;
        long stoneValue = data.SpecDict[difficulty].stoneReward;
        int minTier = data.SpecDict[difficulty].minTier;
        int maxTier = data.SpecDict[difficulty].maxTier;
        float dropRate = data.SpecDict[difficulty].dropRate;
        int dropCount = data.SpecDict[difficulty].dropCount;

        firstGoldText.text = $"{(goldValue * 5).ToString("N0")}";
        firstStoneText.text = $"{(stoneValue * 5).ToString("N0")}";
        // 최초 클리어가 완료되었으면 취소선
        bool isCleared = BossManager.Instance.IsExistClearRecord(BossManager.Instance.currBossID, difficulty);
        if (isCleared)
        {
            firstGoldText.fontStyle = FontStyles.Strikethrough;
            firstStoneText.fontStyle = FontStyles.Strikethrough;
        }
        else
        {
            firstGoldText.fontStyle &= ~FontStyles.Strikethrough;
            firstStoneText.fontStyle &= ~FontStyles.Strikethrough;
        }

        goldText.text = $"{(goldValue).ToString("N0")}";
        stoneText.text = $"{(stoneValue).ToString("N0")}";

        minTierText.text = $"○ 최소 장비 등급 : Tier {minTier}";
        minTierText.color = UIManager.Instance.tierColorList[minTier];
        maxTierText.text = $"○ 최대 장비 등급 : Tier {maxTier}";
        maxTierText.color = UIManager.Instance.tierColorList[maxTier];

        countRateText.text = $"○ 최대 장비 획득 개수 : {dropCount}개 (기본 확률 : {dropRate.ToString("f2")}";
    }

    public void disable()
    {
        upgradeGroup.alpha = 0f;
        upgradeGroup.interactable = false;
        upgradeGroup.blocksRaycasts = false;
    }

    public void enable()
    {
        upgradeGroup.alpha = 1f;
        upgradeGroup.interactable = true;
        upgradeGroup.blocksRaycasts = true;
    }
}
