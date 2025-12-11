using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrowthStackUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GrowthUI parent = null;
    [SerializeField] private Image gaugeBar;
    private RectTransform gaugeRect;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private TextMeshProUGUI stackText;
    [SerializeField] private TextMeshProUGUI restoreCostText;
    [SerializeField] private Button restoreButton;

    [Header("Settings")]
    [SerializeField] private float lerpSpeed = 5f; // 게이지 차오르는 속도
    [SerializeField] private readonly int maxStack = 15;
    [SerializeField] private float maxGaugeWidth = 1216f;

    // 게이지 겉값
    private float targetWidth;

    // 강화 cost
    private long restoreCost = 0;

    // 이 UI가 표시할 스탯 타입 (나중에 업데이트 판별용)
    public StatType TargetStatType { get; private set; }

    private void Awake()
    {
        if (gaugeBar != null)
        {
            gaugeRect = gaugeBar.rectTransform;
        }
    }

    private void Update()
    {
        // 부드러운 게이지 연출
        if (Mathf.Abs(gaugeRect.sizeDelta.x - targetWidth) > 0.1f)
        {
            // Lerp로 새로운 width 계산
            float newWidth = Mathf.Lerp(gaugeRect.sizeDelta.x, targetWidth, Time.deltaTime * lerpSpeed);
            gaugeRect.sizeDelta = new Vector2(newWidth, gaugeRect.sizeDelta.y); // width 적용
        }

        // 비용 텍스트 색 연출
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.gold < restoreCost) { restoreCostText.color = Color.gray; }
            else { restoreCostText.color = Color.white; }
        }
    }

    public void SetParent(GrowthUI _parent)
    {
        parent = _parent;
    }

    public void SetData(StatType type, float value, int currentStack, long _restoreCost)
    {
        string temp;
        TargetStatType = type;

        // 텍스트 정보 갱신
        statText.text = GetStatDisplayName(type) + " : ";

        string typeName = type.ToString();
        if (typeName.EndsWith("Percent"))
        {
            temp = value.ToString("F0") + "%";
        }
        else
        {
            temp = value.ToString("F0");
        }
        statText.text += $"<color=#ffff00>+{temp}</color>";

        // 스택 텍스트 표시 (예: "Lv. 12 / 15")
        if (stackText != null)
        {
            stackText.text = $"Lv. {currentStack} / {maxStack}";
        }

        // 목표 게이지 width 계산
        float ratio = Mathf.Clamp01((float)currentStack / maxStack);
        targetWidth = ratio * maxGaugeWidth;
        if (ratio > (2f/3f)) { gaugeBar.color = new Color(1f, 0.25f, 0.75f); }
        else { gaugeBar.color = Color.white; }

        // 복구 비용 텍스트 표시
        restoreCost = _restoreCost;
        restoreCostText.text = "초기화 비용 : " + restoreCost.ToString("N0");
        
    }

    private string GetStatDisplayName(StatType type)
    {
        if (UIManager.Instance == null)
        {
            Debug.Log("GrowthStackUI : UIManager is null");
            return "(Error)";
        }
        return UIManager.Instance.statNamesList[(int)type].name;
    }

    public void OnClickRestoreButton()
    {
        if (parent == null) { Debug.Log("GrowthStackUI : Parent is null"); return; }
        parent.OnClickRestoreButton(TargetStatType);
    }
}