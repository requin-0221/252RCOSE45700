using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrowthUI : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField] CanvasGroup growthGroup;

    [Header("Item Slot")]
    public Image itemIcon;

    [Header("Info text component")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemLevelText;
    public TextMeshProUGUI itemTierText;
    public TextMeshProUGUI growthCostText;

    [Header("Button")]
    [SerializeField] Button closeButton;
    [SerializeField] Button growthAttemptButton;

    [Header("Stacks field")]
    public Transform container; // Container
    public GameObject statStack; // StatStacks Prefab
    [SerializeField] List<GameObject> statStacks;

    [Header("StatType Name List")]
    public List<StatName> statNames;

    private void Awake()
    {
        if (growthGroup == null) growthGroup = GetComponent<CanvasGroup>();
        
        // 

        // 버튼에 리스너 연결
        closeButton.onClick.AddListener(OnClickCloseButton);
        growthAttemptButton.onClick.AddListener(OnClickUpgradeAttemptButton);

        disable();
    }

    public void disable()
    {
        growthGroup.alpha = 0f;
        growthGroup.interactable = false;
        growthGroup.blocksRaycasts = false;
    }

    public void enable()
    {
        growthGroup.alpha = 1f;
        growthGroup.interactable = true;
        growthGroup.blocksRaycasts = true;
    }

    public void UpdateInfo(ItemInstance item)
    {
        if (item == null)
        {
            Debug.Log("growth UI : item is null");
            return;
        }

        long upgradeCost = (long)Mathf.Floor(UpgradeConfig.Instance.GetUpgradeCost(item.upgradeLv) * Mathf.Sqrt(item.data.itemTier));

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
            growthCostText.text = "강화 비용 : -";
        }
        else
        {
            growthCostText.text = "강화 비용 : " + upgradeCost.ToString("N0");
            if (GameManager.Instance.gold < upgradeCost) { growthCostText.color = Color.gray; }
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
            if (t == null) t = "(Error)";
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
        GameObject lineObj = Instantiate(statStack, container);
        lineObj.GetComponent<TextMeshProUGUI>().text = t;
        lineObj.GetComponent<TextMeshProUGUI>().fontSize = 32;
        statStacks.Add(lineObj);
    }

    void ClearLineTexts()
    {
        foreach (var line in statStacks)
        {
            Destroy(line);
        }
        statStacks.Clear();
    }

    string GetStatTypeName(StatType type)
    {
        if (statNames[(int)type].name != null)
            return statNames[(int)type].name;
        return null;
    }

    private void OnClickCloseButton()
    {
        disable();
    }
}
