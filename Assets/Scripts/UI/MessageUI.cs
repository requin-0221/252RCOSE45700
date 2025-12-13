using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] TextMeshProUGUI buttonText;

    [Header("Button")]
    [SerializeField] Button closeButton;
    [SerializeField] Button messageButton;

    // 콜백 함수를 저장할 변수
    private Action onButtonCallback;
    private Action onCloseCallback;

    private void Awake()
    {
        // 버튼에 리스너 연결
        closeButton.onClick.AddListener(OnCloseClicked);
        messageButton.onClick.AddListener(OnButtonClicked);
    }

    private void Update()
    {
        // ESC - 닫기, Enter(Return) - 버튼 액션
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnCloseClicked();
        }
        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            OnButtonClicked();
        }
    }

    public void Setup(string msg, Action onConfirm = null, Action onCancle = null, string btnTxt = "확인", string title = "Message")
    {
        messageText.text = msg;
        buttonText.text = btnTxt;
        titleText.text = title;

        // 전달받은 콜백 함수 저장
        onButtonCallback = onConfirm;
        onCloseCallback = onCancle;

        // 현재 선택된 포커싱 해제
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnButtonClicked()
    {
        // 콜백 함수가 있으면 실행
        onButtonCallback?.Invoke();
        ClosePopup();
    }

    private void OnCloseClicked()
    {
        // 콜백 함수가 있으면 실행
        onCloseCallback?.Invoke();
        ClosePopup();
    }

    private void ClosePopup()
    {
        Destroy(gameObject); // 팝업 삭제
    }

    public void SetTitleText(string t)
    {
        titleText.text = t;
    }

    public void SetMessageText(string t)
    {
        messageText.text = t;
    }

    public void SetButtonText(string t)
    {
        buttonText.text = t;
    }
}
