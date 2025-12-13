using UnityEngine;
using UnityEngine.UI;

public class BossIllust : MonoBehaviour
{
    [SerializeField] Image bossImage;
    [SerializeField] Image bossHitImage;
    [SerializeField] float alpha = 0f;
    [SerializeField] float alphaSpeed = 0.5f;
    [SerializeField] RectTransform bossAttackEffect;
    [SerializeField] float targetScale = 3f;
    [SerializeField] float currScale;
    [SerializeField] float scaleUpSpeed = 1f;

    void Start()
    {
        Initialze();
    }

    void Update()
    {
        if (currScale < targetScale)
        {
            currScale += Time.deltaTime * scaleUpSpeed;
            bossAttackEffect.localScale = new Vector3(currScale, currScale, 1);
        }

        if (alpha > 0f)
        {
            alpha -= Time.deltaTime * alphaSpeed;
            if (alpha < 0f) alpha = 0f;
            bossHitImage.color = new Color(0.5f, 0f, 0f, alpha);
        }
    }

    public void Initialze()
    {
        if (BossManager.Instance == null)
        {
            Debug.Log("BossManager does not exist.");
            return;
        }

        BossData data = BossManager.Instance.GetCurrentBossData();
        if (data == null)
        {
            Debug.Log("Current Boss Data is null.");
            return;
        }

        bossImage.sprite = bossHitImage.sprite = data.bossSprite;
        bossHitImage.color = new Color(0.5f, 0f, 0f, 0f);
        alpha = 0f;

        bossAttackEffect.localScale = new Vector3(targetScale, targetScale, 1f);
        currScale = targetScale;
    }

    public void OnBossAttack()
    {
        bossAttackEffect.localScale = new Vector3(0f, 0f, 1f);
        currScale = 0f;
    }

    public void OnBossHit()
    {
        alpha = 0.5f;
    }
}
