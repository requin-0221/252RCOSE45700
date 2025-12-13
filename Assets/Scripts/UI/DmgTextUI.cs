using UnityEngine;
using TMPro;

public class DmgTextUI : MonoBehaviour
{
    RectTransform rectTransform;
    TextMeshProUGUI textUI;
    Vector2 moveDirection;
    float moveSpeed;
    bool isMoving = false;
    float alpha = 1f;
    float alphaSpeed = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        textUI = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (alpha > 0f)
        {
            alpha -= Time.deltaTime * alphaSpeed;
            if (alpha < 0f) { Destroy(gameObject); }
            textUI.alpha = alpha;
        }

        if (isMoving) rectTransform.anchoredPosition += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void SetMove(Vector2 direction, float speed, float disappearSpeed)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        isMoving = true;
        alphaSpeed = disappearSpeed;
    }

    public void SetText(string text) { textUI.text = text; }
}
