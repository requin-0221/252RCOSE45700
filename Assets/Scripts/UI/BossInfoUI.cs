using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossInfoUI : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField] CanvasGroup upgradeGroup;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI bossNameText;
    [SerializeField] TextMeshProUGUI bossHpText;
    [SerializeField] TextMeshProUGUI bossShieldText;
    [SerializeField] TextMeshProUGUI bossAtkText;
    [SerializeField] TextMeshProUGUI bossDefText;

    [Header("Boss Illust")]
    [SerializeField] Image bossIllust;

    [Header("Button")]
    [SerializeField] Button difficultyButtonEasy;
    [SerializeField] Button difficultyButtonNormal;
    [SerializeField] Button difficultyButtonHard;
    [SerializeField] Button nextButton;
    [SerializeField] Button prevButton;
    [SerializeField] Button battleButton;

    private void Awake()
    {
        if (upgradeGroup == null) upgradeGroup = GetComponent<CanvasGroup>();
    }

    public void UpdateUI()
    {

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
}
