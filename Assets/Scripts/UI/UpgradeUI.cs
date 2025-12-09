using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

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
    public TextMeshProUGUI UpgradeCostText;

    [Header("Status field")]
    public TextMeshProUGUI UpgradeLevelText;
    public Transform container; // Container
    public GameObject statLine; // StatLine Prefab
    public int row = 17;
    [SerializeField] List<GameObject> statLines;

    [Header("StatType Name List")]
    public List<StatName> statNames;

    private void Awake()
    {
        if (upgradeGroup == null) upgradeGroup = GetComponent<CanvasGroup>();
        
        // StatLine 생성
        for (int i = 0; i < 13; i++)
        {
            GameObject lineObj = Instantiate(statLine, container);
            lineObj.GetComponent<TextMeshProUGUI>().text = null;
            lineObj.GetComponent<TextMeshProUGUI>().fontSize = 32;
            statLines.Add(lineObj);
        }

        disable();
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

        itemIcon.sprite = item.data.icon;
        itemNameText.text = item.data.itemName;
        itemLevelText.text = $"<color=#FF7F00>+{item.upgradeLv}</color> / <color=#00DFFF>+{item.growthLv}</color>";
        itemTierText.text = $"Tier {item.data.itemTier}";
        if (item.data.itemTier <= UIManager.Instance.tierColorList.Count)
        {
            itemTierText.color = UIManager.Instance.tierColorList[item.data.itemTier - 1];
        }
        UpgradeCostText.text = "강화 비용 : " + 
                                (UpgradeConfig.Instance.GetUpgradeCost(item.upgradeLv) 
                                 * Mathf.Sqrt(item.data.itemTier)).ToString("N0");
        // 강화 증가 스탯 부분
        if (item.upgradeLv >= item.data.maxUpgrade)
        {
            UpgradeLevelText.text = $"+{item.upgradeLv} (MAX)";
        }
        else
        {
            UpgradeLevelText.text = $"+{item.upgradeLv}    ▶    <color=#FFFF00>+{item.upgradeLv + 1}</color>";
        }

        UpdateStatLine(item);
    }

    void UpdateStatLine(ItemInstance item)
    {
        int lineIdx = 0;
        string temp = null;

        if (item.upgradeLv >= item.data.maxUpgrade)
        {
            SetLineText(lineIdx++, "최대 강화 단계에 도달했습니다");
            while (lineIdx < 13) SetLineText(lineIdx++, "");
            return;
        }

        foreach (var rule in item.data.upgradeProfile.rules)
        {
            if (lineIdx > 13) break;
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

            SetLineText(lineIdx++, t);
        }

        while (lineIdx < 13) SetLineText(lineIdx++, "");
    }

    void SetLineText(int i, string t)
    {
        statLines[i].GetComponent<TextMeshProUGUI>().text = t;
        return;
    }

    string GetStatTypeName(StatType type)
    {
        if (statNames[(int)type].name != null)
            return statNames[(int)type].name;
        return null;
    }
}
