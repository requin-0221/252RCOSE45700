using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TouchToStart : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private bool isFade = true;
    private Color t_color;

    void Start()
    {
        // TMP_Text 컴포넌트가 할당되었는지 확인
        if (text == null)
        {
            Debug.LogError("TMP_Text component is not assigned. Please assign it in the Inspector.");
            return;
        }

        // 텍스트의 원래 색상(RGB)을 저장합니다.
        // 불투명도(A)만 변경하기 위함입니다.
        t_color = text.color;

        // 불투명도 변경 코루틴 시작
        StartCoroutine(ChangeTextAlpha());
    }


    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            transform.GetComponent<SceneTransition>().StartFadeOut("MainScene");
        }
    }

    // 텍스트 불투명도를 변경하는 코루틴
    IEnumerator ChangeTextAlpha()
    {
        while (true)
        {
            float currentAlpha = text.color.a;

            if (isFade)
            {
                // 불투명도 증가 (페이드 인)
                currentAlpha += Time.deltaTime;
                if (currentAlpha >= 1f)
                {
                    currentAlpha = 1f;
                    isFade = false; // 불투명도가 1에 도달하면 감소로 전환
                }
            }
            else
            {
                // 불투명도 감소 (페이드 아웃)
                currentAlpha -= Time.deltaTime;
                if (currentAlpha <= 0f)
                {
                    currentAlpha = 0f;
                    isFade = true; // 불투명도가 0에 도달하면 증가로 전환
                }
            }

            Color newColor = new Color(t_color.r, t_color.g, t_color.b, currentAlpha);
            text.color = newColor;

            yield return null;
        }
    }
}