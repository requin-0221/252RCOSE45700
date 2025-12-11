using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct StatName
{
    public StatType statType;
    public string name;
}

[System.Serializable]
public struct EquipSlotName
{
    public EquipSlot equipSlot;
    public string name;
}

public class Iteminfo : MonoBehaviour
{
    public TextMeshProUGUI emptyText; //empty slot text
    public CanvasGroup infoGroup;

    [Header("Info Slot")]
    public Image itemIcon;

    [Header("Info text component")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemLevelText;
    public TextMeshProUGUI itemTierText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI TakeButtonText;
    public TextMeshProUGUI SellButtonText;

    [Header("Status field")]
    public Transform container; // Container
    public GameObject statLine; // StatLine Prefab
    public int row; // # of Container element
    public int col;
    [SerializeField] List<GameObject> statLines;

    [Header("EquipSlot Name List")]
    [SerializeField] List<EquipSlotName> equipSlotNames;

    public void Start()
    {
        for (int i = 0; i < row * col; i++)
        {
            GameObject lineObj = Instantiate(statLine, container);
            lineObj.GetComponent<TextMeshProUGUI>().text = null;
            statLines.Add(lineObj);
        }

        ShowEmpty();
    }

    public void UpdateInfo(ItemInstance item)
    {
        if (item == null)
        {
            ShowEmpty();
            return;
        }

        ShowItemInfo(item);
    }

    private void ShowEmpty()
    {
        emptyText.gameObject.SetActive(true);
        infoGroup.alpha = 0; // 0으로 하면 숨겨지고
        infoGroup.interactable = false; // 버튼 클릭도 막음
    }

    private void ShowItemInfo(ItemInstance item)
    {
        emptyText.gameObject.SetActive(false);
        infoGroup.alpha = 1;
        infoGroup.interactable = true;

        // 아이템 정보 업데이트
        itemIcon.sprite = item.data.icon;
        itemNameText.text = item.data.itemName;
        itemLevelText.text = $"<color=#FF7F00>+{item.upgradeLv}</color> / <color=#00DFFF>+{item.growthLv}</color>";
        itemTierText.text = $"Tier {item.data.itemTier}";
        if (item.data.itemTier <= UIManager.Instance.tierColorList.Count)
        {
            itemTierText.color = UIManager.Instance.tierColorList[item.data.itemTier-1];
        }
        itemPriceText.text = "판매가 : " + item.GetSellPrice().ToString("N0");

        // 장착 중인지 여부 체크
        if (InventoryManager.Instance.equipments.ContainsValue(item))
        {
            TakeButtonText.text = "해제";
            SellButtonText.color = Color.gray;
        }
        else
        {
            TakeButtonText.text = "장착";
            SellButtonText.color = Color.white;
        }

        UpdateStatLine(item);
    }

    void UpdateStatLine(ItemInstance item)
    {
        int lineIdx = 0;
        string temp = null;
        // 1번 열
        SetLineText(lineIdx++, "장비 분류 : " + GetEquipSlotName(item.data.equipSlot));
        if (item.data.equipSlot == EquipSlot.Weapon)
        {
            string attackType = item.data.attackType == AttackType.Physical ? "물리" : "마법";
            SetLineText(lineIdx++, "공격 유형 : " + attackType);

            temp = GetStatInfoText(item, StatType.AtkSpdPercent);
            if (temp != null) SetLineText(lineIdx++, temp); // 공격 속도
        }
        // 단순 수치들 (HP, 방어력, 물리 공격력, 마법 공격력)
        for (int i = 1; i <= 7; i++)
        {
            temp = GetStatInfoText(item, (StatType)i);
            if (temp == null) continue;
            SetLineText(lineIdx++, temp);
        }
        while (lineIdx < row)
        {
            SetLineText(lineIdx++, "");
        }

        // 2번 열
        for (int i = 8; i < Enum.GetNames(typeof(StatType)).Length; i++)
        {
            temp = GetStatInfoText(item, (StatType)i);
            if (temp == null) continue;
            SetLineText(lineIdx++, GetStatInfoText(item, (StatType)i));
        }
        while (lineIdx < row*2)
        {
            SetLineText(lineIdx++, "");
        }
    }

    void SetLineText(int i, string t)
    {
        statLines[i].GetComponent<TextMeshProUGUI>().text = t;
        return;
    }

    string GetStatInfoText(ItemInstance item, StatType type)
    {
        int i = (int)type;
        // 수치가 없는 능력치는 스킵
        if (item.totalStats[i] <= 0f) return null;

        // 스탯 이름
        string t = GetStatTypeName(type);
        t += " : ";

        // Total_Stat(+Upgrade_Stat+Growth_Stat) 포맷
        string upgradeStr, growthStr, totalStr;
        string typeName = type.ToString(); // "PhysicalAttack", "CritDamagePercent"
        string format = "F0"; // 소수점 없음

        if (typeName.EndsWith("Percent"))
        {
            //baseStr = (item.baseStats[i] * 100).ToString(format) + "%";
            upgradeStr = (item.upgradeStats[i]).ToString(format) + "%";
            growthStr = (item.growthStats[i]).ToString(format) + "%";
            totalStr = (item.totalStats[i]).ToString(format) + "%";
        }
        else
        {
            //baseStr = item.baseStats[i].ToString(format);
            upgradeStr = item.upgradeStats[i].ToString(format);
            growthStr = item.growthStats[i].ToString(format);
            totalStr = item.totalStats[i].ToString(format);
        }

        t += $"+{totalStr}";
        string inside_t = "";
        if (item.upgradeStats[(int)type] > 0f)
            inside_t += $"<color=#ff7f00>+{upgradeStr}</color>";
        if (item.growthStats[(int)type] > 0f)
            inside_t += $"<color=#00dfff>+{growthStr}</color>";
        if (inside_t.Length > 0)
            t += "(" + inside_t + ")";

        return t;
    }

    string GetEquipSlotName(EquipSlot type)
    {
        if (equipSlotNames[(int)type].name != null)
            return equipSlotNames[(int)type].name;
        return null;
    }

    private string GetStatTypeName(StatType type)
    {
        if (UIManager.Instance == null)
        {
            Debug.Log("ItemInfo : UIManager is null");
            return "(Error)";
        }
        return UIManager.Instance.statNamesList[(int)type].name;
    }

    public void OnClickSellButton()
    {
        if (UIManager.Instance == null || InventoryManager.Instance == null)
        {
            Debug.Log("UIManger or InventoryManager is null");
            return;
        }

        // 아이템 확인
        ItemSlot targetSlot = UIManager.Instance.CurrSlot;
        ItemInstance targetItem = UIManager.Instance.CurrSlot._item;

        if (targetItem == null)
        {
            Debug.Log("빈 슬롯 오류");
            return;
        }

        // 장착 중인 아이템은 판매 불가 처리
        if (InventoryManager.Instance.equipments.ContainsValue(targetItem))
        {
            Debug.Log("장착 중인 아이템은 판매할 수 없습니다.");
            UIManager.Instance.ShowPopup("장착 중인 아이템은 판매할 수 없습니다.");
            return;
        }

        InventoryManager.Instance.SellItem(targetItem);
    }

    public void OnClickTakeButton()
    {
        if (UIManager.Instance == null || InventoryManager.Instance == null)
        {
            Debug.Log("UIManger or InventoryManager is null");
            return;
        }

        // 아이템 확인
        ItemSlot targetSlot = UIManager.Instance.CurrSlot;
        ItemInstance targetItem = UIManager.Instance.CurrSlot._item;

        if (targetItem == null)
        {
            Debug.Log("빈 슬롯 오류");
            return;
        }

        // 장착 중인 아이템은 해제
        if (InventoryManager.Instance.equipments.ContainsValue(targetItem))
        {
            InventoryManager.Instance.ReleaseItem(targetSlot);
        }
        else
        {
            // 아니라면 장착
            InventoryManager.Instance.EquipItem(targetSlot);
        }
        return;
    }

    public void OnClickUpgradeButton()
    {
        if (UIManager.Instance != null) UIManager.Instance.ShowUpgradeUI();
    }

    public void OnClickGrowthButton()
    {
        if (UIManager.Instance != null) UIManager.Instance.ShowGrowthUI();
    }
}
