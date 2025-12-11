using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField] CanvasGroup upgradeGroup;

    [Header("Item Slot")]
    public Image itemIcon;

    [Header("Info text component")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemLevelText;
    public TextMeshProUGUI itemTierText;
    public TextMeshProUGUI upgradeCostText;

    [Header("Button")]
    [SerializeField] Button closeButton;
    [SerializeField] Button upgradeAttemptButton;

    [Header("Status field")]
    public TextMeshProUGUI upgradeLevelText;
    public TextMeshProUGUI upgradeProbText;
    public Transform container; // Container
    public GameObject statLine; // StatLine Prefab
    [SerializeField] List<GameObject> statLines;

    private long upgradeCost = 0;

    private void Awake()
    {
        if (upgradeGroup == null) upgradeGroup = GetComponent<CanvasGroup>();
        
        // StatLine 생성
        for (int i = 0; i < 11; i++)
        {
            GameObject lineObj = Instantiate(statLine, container);
            lineObj.GetComponent<TextMeshProUGUI>().text = null;
            lineObj.GetComponent<TextMeshProUGUI>().fontSize = 32;
            statLines.Add(lineObj);
        }

        // 버튼에 리스너 연결
        closeButton.onClick.AddListener(OnClickCloseButton);
        upgradeAttemptButton.onClick.AddListener(OnClickUpgradeAttemptButton);

        disable();
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.gold < upgradeCost) { upgradeCostText.color = Color.gray; }
            else { upgradeCostText.color = Color.white; }
        }
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

    public void UpdateInfo(ItemInstance item)
    {
        if (item == null)
        {
            Debug.Log("Upgrade UI : item is null");
            return;
        }

        upgradeCost = (long)Mathf.Floor(UpgradeConfig.Instance.GetUpgradeCost(item.upgradeLv) * Mathf.Sqrt(item.data.itemTier));

        itemIcon.sprite = item.data.icon;
        itemNameText.text = item.data.itemName;
        itemLevelText.text = $"<color=#FF7F00>+{item.upgradeLv}</color> / <color=#00DFFF>+{item.growthLv}</color>";
        itemTierText.text = $"Tier {item.data.itemTier}";
        if (item.data.itemTier <= UIManager.Instance.tierColorList.Count)
        {
            itemTierText.color = UIManager.Instance.tierColorList[item.data.itemTier - 1];
        }
        if (item.upgradeLv >= item.data.maxUpgrade)
        {
            upgradeCost = 0;
            upgradeCostText.text = "강화 비용 : -";
            upgradeProbText.text = "-";
            upgradeLevelText.text = $"+{item.upgradeLv} (MAX)";
        }
        else
        {
            upgradeCostText.text = "강화 비용 : " + upgradeCost.ToString("N0");
            upgradeProbText.text = "성공 확률 : " + (UpgradeConfig.Instance.GetSuccessProb(item.upgradeLv)*100).ToString("F0") + "%";
            upgradeLevelText.text = $"+{item.upgradeLv}    ▶    <color=#FFFF00>+{item.upgradeLv + 1}</color>";
        }

        UpdateStatLine(item);
    }

    void OnClickUpgradeAttemptButton()
    {
        if (UIManager.Instance == null || InventoryManager.Instance == null)
        {
            Debug.Log("UIManager or InventoryManager is null");
            return;
        }

        // 아이템 확인
        ItemInstance item = UIManager.Instance.CurrSlot._item;

        if (item == null)
        {
            Debug.Log("빈 슬롯 오류");
            return;
        }

        if (item.upgradeLv >= item.data.maxUpgrade)
        {
            UIManager.Instance.ShowPopup("더 이상 강화할 수 없습니다.");
            return;
        }

        long upgradeCost = item.GetUpgradeCost();

        if (GameManager.Instance.gold < upgradeCost)
        {
            UIManager.Instance.ShowPopup("재화가 부족합니다.");
            return;
        }

        if (InventoryManager.Instance.UpgradeAttempt(item)) // 강화 성공
        {
            UIManager.Instance.ShowPopup("강화가 성공했습니다.");
        } else
        {
            UIManager.Instance.ShowPopup("강화가 실패했습니다.");
        }
    }

    void UpdateStatLine(ItemInstance item)
    {
        ClearLineTexts();

        if (item.upgradeLv >= item.data.maxUpgrade)
        {
            CreateLineText("최대 강화 단계에 도달했습니다");
            return;
        }

        string temp = null;
        foreach (var rule in item.data.upgradeProfile.rules)
        {
            StatType _type = rule.statType;
            float value = rule.values[item.upgradeLv];

            if (value == 0) continue;

            if (_type == StatType.MaxHp || _type == StatType.Def ||
                _type == StatType.PAtk || _type == StatType.MAtk)
            {
                value = Mathf.Floor(value * Mathf.Sqrt(item.data.itemTier));
            }

            // 스탯 이름
            string t = GetStatTypeName(_type);
            t += " : ";

            string typeName = _type.ToString();
            if (typeName.EndsWith("Percent"))
            {
                temp = value.ToString("F0") + "%";
            }
            else
            {
                temp = value.ToString("F0");
            }
            t += $"<color=#ffff00>+{temp}</color>";

            CreateLineText(t);
        }
    }

    void CreateLineText(string t)
    {
        GameObject lineObj = Instantiate(statLine, container);
        lineObj.GetComponent<TextMeshProUGUI>().text = t;
        lineObj.GetComponent<TextMeshProUGUI>().fontSize = 32;
        statLines.Add(lineObj);
    }

    void ClearLineTexts()
    {
        foreach (var line in statLines)
        {
            Destroy(line);
        }
        statLines.Clear();
    }

    private string GetStatTypeName(StatType type)
    {
        if (UIManager.Instance == null)
        {
            Debug.Log("UpgradeUI : UIManager is null");
            return "(Error)";
        }
        return UIManager.Instance.statNamesList[(int)type].name;
    }

    private void OnClickCloseButton()
    {
        disable();
    }
}
