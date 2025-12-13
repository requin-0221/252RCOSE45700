using TMPro;
using UnityEngine;

public class PlayerGaugeUI : MonoBehaviour
{
    [Header("Gauge")]
    [SerializeField] RectTransform HpBar;
    [SerializeField] RectTransform SpBar;
    [SerializeField] float lerpSpeed = 1.0f;
    public float targetWidth_HP;
    const float originWidth = 672f;
    public float targetWidth_SP;
    [SerializeField] TextMeshProUGUI HpBarText;
    [SerializeField] TextMeshProUGUI SpBarText;

    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        // 체력바 부드럽게 변화
        if (Mathf.Abs(HpBar.sizeDelta.x - targetWidth_HP) > 0.1f)
        {
            float newWidth = Mathf.Lerp(HpBar.sizeDelta.x, targetWidth_HP, Time.deltaTime * lerpSpeed);
            HpBar.sizeDelta = new Vector2(newWidth, HpBar.sizeDelta.y); // width 적용
        }

        // SP바 부드럽게 변화
        if (Mathf.Abs(SpBar.sizeDelta.x - targetWidth_SP) > 0.1f)
        {
            float newWidth = Mathf.Lerp(SpBar.sizeDelta.x, targetWidth_SP, Time.deltaTime * lerpSpeed);
            SpBar.sizeDelta = new Vector2(newWidth, SpBar.sizeDelta.y); // width 적용
        }
    }

    public void UpdateUI()
    {
        if (BattleManager.Instance == null)
        {
            Debug.Log("BattleManager does not exist.");
            return;
        }

        float HpRatio = BattleManager.Instance.GetPlayerHpRatio();
        float SpRatio = BattleManager.Instance.GetSkillGaugeRatio();
        HpBarText.text = $"HP : {BattleManager.Instance.playerHP} / {BattleManager.Instance.playerMaxHP}";
        SpBarText.text = $"Skill : {(SpRatio*100f).ToString("F2")}%";

        targetWidth_HP = originWidth * HpRatio;
        targetWidth_SP = originWidth * SpRatio;
    }

    public void Initialize()
    {
        if (BattleUIManager.Instance != null) return;

        HpBarText.text = $"HP : {BattleManager.Instance.playerHP} / {BattleManager.Instance.playerMaxHP}";
        SpBarText.text = $"Skill : 0.00%";

        targetWidth_HP = originWidth;
        targetWidth_SP = 0f;
    }
}
