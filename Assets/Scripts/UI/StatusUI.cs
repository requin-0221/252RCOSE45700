using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    public Transform container; // Container
    public GameObject statLine; // StatLine Prefab
    public int row; // # of Container element
    public int col;
    [SerializeField] List<GameObject> statLines;

    [SerializeField] Color highlight_color;

    void Awake()
    {
        for (int i = 0; i < row * col; i++)
        {
            GameObject lineObj = Instantiate(statLine, container);
            lineObj.GetComponent<TextMeshProUGUI>().text = null;
            statLines.Add(lineObj);
        }
    }

    public void UpdateUI()
    {
        int lineIdx = 0;
        string temp = null;
        bool isPhysicalAttack = PlayerStat.Instance.FinalAttackType == AttackType.Physical;

        // status line setting
        temp = isPhysicalAttack ? "물리" : "마법";
        SetLineText(lineIdx++, "공격 유형 : " + temp);
        SetLineText(lineIdx++, "최대 HP : " + PlayerStat.Instance.FinalMaxHp.ToString("N0"));
        SetLineText(lineIdx++, "방어력 : " + PlayerStat.Instance.FinalDef.ToString("N0"));
        SetLineText(lineIdx++, "물리 공격력 : " + PlayerStat.Instance.FinalPAtk.ToString("N0"), isPhysicalAttack);
        SetLineText(lineIdx++, "마법 공격력 : " + PlayerStat.Instance.FinalMAtk.ToString("N0"), !isPhysicalAttack);
        SetLineText(lineIdx++, "물리 피해량% : +" + PlayerStat.Instance.FinalPDmgPercent.ToString("F2") + "%", isPhysicalAttack);
        SetLineText(lineIdx++, "마법 피해량% : +" + PlayerStat.Instance.FinalMDmgPercent.ToString("F2") + "%", !isPhysicalAttack);
        SetLineText(lineIdx++, "추가 피해량% : +" + PlayerStat.Instance.FinalDmgPercent.ToString("F2") + "%");

        SetLineText(lineIdx++, "공격 속도% : +" + PlayerStat.Instance.FinalAtkSpdPercent.ToString("F2") + "%");
        SetLineText(lineIdx++, "치명타 확률% : " + PlayerStat.Instance.FinalCriRate.ToString("F2") + "%");
        SetLineText(lineIdx++, "치명타 피해량% : +" + PlayerStat.Instance.FinalCriDmgPercent.ToString("F2") + "%");
        SetLineText(lineIdx++, "방어 관통률% : " + PlayerStat.Instance.FinalDefPntr.ToString("F2") + "%");
        SetLineText(lineIdx++, "스킬 피해량% : " + PlayerStat.Instance.FinalSkillDmg.ToString("F2") + "%");
        SetLineText(lineIdx++, "스킬 자원 획득% : +" + PlayerStat.Instance.FinalSkillGainPercent.ToString("F2") + "%");
        SetLineText(lineIdx++, "아이템 획득% : +" + PlayerStat.Instance.FinalItemDropPercent.ToString("F2") + "%");
        SetLineText(lineIdx++, "재화 획득% : +" + PlayerStat.Instance.FinalGoldDropPercent.ToString("F2") + "%");
    }

    void SetLineText(int i, string t, bool isHighlight = false)
    {
        statLines[i].GetComponent<TextMeshProUGUI>().text = t;
        if (isHighlight) statLines[i].GetComponent<TextMeshProUGUI>().color = highlight_color;
        return;
    }
}
