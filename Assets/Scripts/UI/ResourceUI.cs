using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    [Header("리소스 UI 요소")]
    public Image goldIcon;
    public TextMeshProUGUI goldText;
    public Image stoneIcon;
    public TextMeshProUGUI stoneText;

    public void UpdateUI()
    {
        goldText.text = GameManager.Instance.gold.ToString("N0");
        stoneText.text = GameManager.Instance.stones.ToString("N0");
    }
}
