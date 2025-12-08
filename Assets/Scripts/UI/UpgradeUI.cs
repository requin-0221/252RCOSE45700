using System;
using System.Collections.Generic;
using TMPro;
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
        UpgradeLevelText.text = $"+{item.upgradeLv}    ▶    <color=#FFFF00>+{item.upgradeLv + 1}</color>";

    }
}
