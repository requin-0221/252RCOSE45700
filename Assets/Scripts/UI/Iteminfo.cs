using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Iteminfo : MonoBehaviour
{
    public TextMeshProUGUI emptyText; //empty slot text
    public CanvasGroup infoGroup;

    [Header("Info text component")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemLevelText;
    public TextMeshProUGUI itemTierText;

    [Header("Status field")]
    public Transform container; // Content
    public GameObject statLine; // StatLine Prefab

    public void Start()
    {
        emptyText.gameObject.SetActive(false);
    }

    public void UpdatePanel(ItemInstance item)
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
        itemNameText.text = item.data.itemName;
        itemTierText.text = $"Tier {item.data.itemTier}";
        itemLevelText.text = $"+{item.upgradeLv} / +{item.growthLv}";

        // 스탯 라인 컨테이너
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        AddStatLine("장비 유형", (int)item.data.equipSlot);
        AddStatLine("최대 HP", item.)
    }

    private void AddStatLine(string statName, long value)
    {
        GameObject lineObj = Instantiate(statLine, container);
        TextMeshProUGUI statText = lineObj.GetComponent<TextMeshProUGUI>();

        // 값이 0이면 
        if (value == 0) statText.text = null;

    }

    // (오버로딩) float 값(%)을 위한 함수
    private void AddStatLine(string statName, float value, bool isPercent = false)
    {
        if (value == 0) return;

        GameObject lineObj = Instantiate(statLinePrefab, statContainer);
        var texts = lineObj.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = statName;
        texts[1].text = isPercent ? $"{value:P2}" : value.ToString("N1"); // 예: 30.40%
    }
}
