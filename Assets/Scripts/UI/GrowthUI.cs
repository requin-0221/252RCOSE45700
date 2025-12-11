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
    public TextMeshProUGUI entireRestoreCostText;

    [Header("Button")]
    [SerializeField] Button closeButton;
    [SerializeField] Button growthAttemptButton;
    [SerializeField] Button entireRestoreButton;

    [Header("Stacks field")]
    public Transform container; // Container
    public GrowthStackUI statStack; // StatStacks Prefab
    public List<GrowthStackUI> statStacks;

    private int growthCost = 0;
    private long entireRestoreCost = 0;

    private void Awake()
    {
        if (growthGroup == null) growthGroup = GetComponent<CanvasGroup>();
        
        // 버튼에 리스너 연결
        closeButton.onClick.AddListener(OnClickCloseButton);
        growthAttemptButton.onClick.AddListener(OnClickGrowthAttemptButton);
        entireRestoreButton.onClick.AddListener(OnClickEntireRestoreButton);

        disable();
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.gold < entireRestoreCost) { entireRestoreCostText.color = Color.gray; }
            else { entireRestoreCostText.color = Color.white; }

            if (GameManager.Instance.gold < growthCost) { growthCostText.color = Color.gray; }
            else { growthCostText.color = Color.white; }
        }
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

        // 성장 비용 계산
        growthCost = item.GetGrowthCost();
        entireRestoreCost = item.GetEntireRestoreCost();

        itemIcon.sprite = item.data.icon;
        itemNameText.text = item.data.itemName;
        itemLevelText.text = $"<color=#FF7F00>+{item.upgradeLv}</color> / <color=#00DFFF>+{item.growthLv}</color>";
        itemTierText.text = $"Tier {item.data.itemTier}";
        if (item.data.itemTier <= UIManager.Instance.tierColorList.Count)
        {
            itemTierText.color = UIManager.Instance.tierColorList[item.data.itemTier - 1];
        }

        if (item.growthLv >= item.data.maxGrowth)
        {
            growthCostText.text = "필요 마석 : -";
        }
        else
        {
            growthCostText.text = "필요 마석 : " + growthCost.ToString("N0");
        }

        if (item.growthLv == 0)
        {
            entireRestoreCostText.text = "전체 초기화 비용 : -";
        }
        else
        {
            entireRestoreCostText.text = "전체 초기화 비용 : " + entireRestoreCost.ToString("N0");
        }

        UpdateStatStacks(item);
    }

    void OnClickGrowthAttemptButton()
    {
        if (UIManager.Instance == null || InventoryManager.Instance == null)
        {
            Debug.Log("GrowthUI : UIManager or InventoryManager is null");
            return;
        }

        // 아이템 확인
        ItemInstance item = UIManager.Instance.CurrSlot._item;

        if (item == null)
        {
            Debug.Log("빈 슬롯 오류");
            return;
        }

        if (item.growthLv >= item.data.maxGrowth)
        {
            UIManager.Instance.ShowPopup("최대 성장 단계에 도달했습니다.");
            return;
        }

        int cost = item.GetGrowthCost();

        if (GameManager.Instance.stone < cost)
        {
            UIManager.Instance.ShowPopup("마석이 부족합니다.");
            return;
        }

        InventoryManager.Instance.GrowingAttempt(item);
    }

    void UpdateStatStacks(ItemInstance item)
    {
        if (statStacks.Count < item.growthStacks.Count)
        {
            for (int i = statStacks.Count; i < item.growthStacks.Count; i++)
            {
                GrowthStackUI stackObj = Instantiate(statStack, container);
                stackObj.SetParent(this);
                statStacks.Add(stackObj);
            }
        }
        else
        {
            for (int i = statStacks.Count - 1; i >= item.growthStacks.Count; i--)
            {
                Destroy(statStacks[i].gameObject);
                statStacks.RemoveAt(i);
            }
        }

        int n = 0;

        foreach (var stack in item.growthStacks)
        {
            StatType _type = stack.Key;
            int stackNum = stack.Value;
            float value = item.growthStats[(int)_type];

            /*
            if (_type == StatType.MaxHp || _type == StatType.Def ||
                _type == StatType.PAtk || _type == StatType.MAtk)
            {
                value = Mathf.Floor(value * Mathf.Sqrt(item.data.itemTier));
            }
            */

            statStacks[n++].SetData(_type, value, stackNum, item.GetGrowthStackRestoreCost(stackNum));
        }
    }

    public bool OnClickRestoreButton(StatType type)
    {
        if (UIManager.Instance == null || InventoryManager.Instance == null)
        {
            Debug.Log("GrowthUI : UIManager or InventoryManager is null");
            return false;
        }

        // 아이템 확인
        ItemInstance item = UIManager.Instance.CurrSlot._item;

        if (item == null)
        {
            Debug.Log("빈 슬롯 오류");
            return false;
        }

        int stackNum = item.growthStacks[type];

        long restoreCost = item.GetGrowthStackRestoreCost(stackNum);

        if (GameManager.Instance.gold < restoreCost)
        {
            UIManager.Instance.ShowPopup("재화가 부족합니다.");
            return false;
        }

        InventoryManager.Instance.GrowthStackRestore(item, type);
        UIManager.Instance.ShowPopup("선택한 성장 스택이 초기화되었습니다.");
        return true;
    }

    public void OnClickEntireRestoreButton()
    {
        if (UIManager.Instance == null || InventoryManager.Instance == null)
        {
            Debug.Log("GrowthUI : UIManager or InventoryManager is null");
            return;
        }

        // 아이템 확인
        ItemInstance item = UIManager.Instance.CurrSlot._item;

        if (item == null)
        {
            Debug.Log("빈 슬롯 오류");
            return;
        }

        if (item.growthLv <= 0)
        {
            UIManager.Instance.ShowPopup("아직 성장하지 않은 아이템입니다.");
            return;
        }

        long restoreCost = item.GetEntireRestoreCost();

        if (GameManager.Instance.gold < restoreCost)
        {
            UIManager.Instance.ShowPopup("재화가 부족합니다.");
            return;
        }

        InventoryManager.Instance.GrowthEntireRestore(item);
        UIManager.Instance.ShowPopup("모든 성장 스택이 초기화되었습니다.");
    }

    private void OnClickCloseButton()
    {
        disable();
    }
}
