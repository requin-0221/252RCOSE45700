using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DifficultyImagePair
{
    public BossDifficulty difficulty;
    public Sprite sprite;
}

public class BossGaugeUI : MonoBehaviour
{
    [Header("UI component")]
    [SerializeField] Image bossIcon;
    [SerializeField] Image DifficultyUI;

    [Header("difficulty Image")]
    [SerializeField] List<DifficultyImagePair> difficultyImagePairs;

    [Header("Gauge")]
    [SerializeField] RectTransform bossHpBar;
    [SerializeField] RectTransform bossShieldBar;
    [SerializeField] float lerpSpeed = 1.0f;
    public float targetWidth_HP;
    const float originWidth_HP = 1654f;
    public float targetWidth_Shield;
    const float originWidth_Shield = 1600f;
    [SerializeField] TextMeshProUGUI bossHpText;

    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        // 보스 체력바 부드럽게 변화
        if (Mathf.Abs(bossHpBar.sizeDelta.x - targetWidth_HP) > 0.1f)
        {
            float newWidth = Mathf.Lerp(bossHpBar.sizeDelta.x, targetWidth_HP, Time.deltaTime * lerpSpeed);
            bossHpBar.sizeDelta = new Vector2(newWidth, bossHpBar.sizeDelta.y); // width 적용
        }

        // 보스 쉴드 부드럽게 변화
        if (Mathf.Abs(bossShieldBar.sizeDelta.x - targetWidth_Shield) > 0.1f)
        {
            float newWidth = Mathf.Lerp(bossShieldBar.sizeDelta.x, targetWidth_Shield, Time.deltaTime * lerpSpeed);
            bossShieldBar.sizeDelta = new Vector2(newWidth, bossShieldBar.sizeDelta.y); // width 적용
        }
    }

    public void Initialize()
    {
        if (BossManager.Instance == null)
        {
            Debug.Log("BossManager does not exist.");
            return;
        }

        BossData data = BossManager.Instance.GetCurrentBossData();
        BossDifficulty difficulty = BossManager.Instance.difficulty;
        if (data == null)
        {
            Debug.Log("Current Boss Data is null.");
            return;
        }

        bossIcon.sprite = data.bossIcon;
        foreach (var pair in difficultyImagePairs)
        {
            if (difficulty == pair.difficulty)
            {
                DifficultyUI.sprite = pair.sprite;
                break;
            }
        }
        targetWidth_HP = originWidth_HP;
        targetWidth_Shield = originWidth_Shield;
        bossHpText.text = "100.00%";
    }

    public void UpdateUI()
    {
        if (BattleManager.Instance == null) return;

        float hpRatio = BattleManager.Instance.GetHpRatio();
        targetWidth_HP = originWidth_HP * hpRatio;
        bossHpText.text = (hpRatio*100f).ToString("F2") + "%";
        targetWidth_Shield = originWidth_Shield * BattleManager.Instance.GetShieldRatio();
    }
}
