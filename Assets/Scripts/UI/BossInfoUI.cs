using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DifficultyButtonPair
{
    public BossDifficulty difficulty;
    public Button button;
}

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
    [SerializeField] Image bossImage;

    [Header("Button")]
    [SerializeField] List<DifficultyButtonPair> difficultyButtons;
    [SerializeField] Button nextButton;
    [SerializeField] Button prevButton;
    [SerializeField] Button battleButton;

    private void Awake()
    {
        if (upgradeGroup == null) upgradeGroup = GetComponent<CanvasGroup>();

        foreach (var pair in difficultyButtons)
        {
            // 람다식에서 pair 변수를 직접 쓰면 클로저 문제가 생길 수 있어 로컬 복사
            BossDifficulty diff = pair.difficulty;

            pair.button.onClick.AddListener(() => OnClickDifficultyButton(diff));
        }
        if (nextButton) nextButton.onClick.AddListener(OnClickNextBoss);
        if (prevButton) prevButton.onClick.AddListener(OnClickPrevBoss);
        if (battleButton) battleButton.onClick.AddListener(OnClickBattleButton);
    }

    public void UpdateUI()
    {
        if (BossManager.Instance == null) return;

        BossData data = BossManager.Instance.GetCurrentBossData();
        if (data == null) return;

        bossImage.sprite = data.bossSprite;
        bossNameText.text = data.bossName;

        bool isCurrentDiffValid = false;
        BossDifficulty difficulty = BossManager.Instance.difficulty;

        // 현재 보스의 보유 난이도에 따른 난이도 버튼 노출
        foreach (var pair in difficultyButtons)
        {
            bool hasKey = data.SpecDict.ContainsKey(pair.difficulty);

            pair.button.GetComponent<Image>().color = Color.gray;
            pair.button.gameObject.SetActive(hasKey);

            // 현재 선택된 난이도가 유효한지 체크
            if (hasKey && BossManager.Instance.difficulty == pair.difficulty)
            {
                isCurrentDiffValid = true;
                pair.button.GetComponent<Image>().color = Color.white;
            }
        }

        // 현재 선택된 난이도가 보스한테 없다면 강제 변경
        if (!isCurrentDiffValid)
        {
            // 사용 가능한 첫 번째 난이도로 강제 변경
            foreach (var pair in difficultyButtons)
            {
                if (data.SpecDict.ContainsKey(pair.difficulty))
                {
                    BossManager.Instance.difficulty = pair.difficulty;
                    break;
                }
            }
        }

        // 현재 BossID가 양 끝값이면 Next / Prev 버튼 비활성화
        prevButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);

        if (BossManager.Instance.currBossID == 0)
        {
            prevButton.gameObject.SetActive(false);
        }
        if (BossManager.Instance.currBossID == BossManager.Instance.bossList.Count-1)
        {
            nextButton.gameObject.SetActive(false);
        }

        UpdateText(data, difficulty);
    }

    void UpdateText(BossData data, BossDifficulty difficulty)
    {
        if (data.SpecDict.TryGetValue(difficulty, out BossSpec spec))
        {
            bossHpText.text = $"HP : {spec.maxHp:N0}";
            bossShieldText.text = $"보호막 : {spec.maxShield:N0}";
            bossAtkText.text = $"공격력 : {spec.atk:N0}";
            bossDefText.text = $"방어력 : {spec.def:N0}";
        }
        else
        {
            // 해당 난이도 데이터가 없는 경우 (예: Easy 없는 보스)
            bossHpText.text = $"HP : -";
            bossShieldText.text = $"보호막 : -";
            bossAtkText.text = $"공격력 : -";
            bossDefText.text = $"방어력 : -";
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

    void OnClickDifficultyButton(BossDifficulty difficulty)
    {
        BossManager.Instance.SetDifficulty(difficulty);
        UIManager.Instance.RefreshAllUI();
    }

    public void OnClickNextBoss()
    {
        BossManager.Instance.SetCurrentBossID(BossManager.Instance.currBossID + 1);
        UIManager.Instance.RefreshAllUI();
    }

    public void OnClickPrevBoss()
    {
        BossManager.Instance.SetCurrentBossID(BossManager.Instance.currBossID - 1);
        UIManager.Instance.RefreshAllUI();
    }

    public void OnClickBattleButton()
    {
        Debug.Log("보스 전투 진입");
        if (UIManager.Instance.sceneTransition == null)
        {
            Debug.Log("No scene transition");
            return;
        }
        UIManager.Instance.sceneTransition.StartFadeOut("BossScene");
    }
}
