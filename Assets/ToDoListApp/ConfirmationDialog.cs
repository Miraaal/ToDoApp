using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 확인/취소 다이얼로그
/// 삭제, 초기화 등 중요한 작업에 대한 사용자 확인을 받습니다.
/// </summary>
public class ConfirmationDialog : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text titleText;        // 제목 (예: "루틴 삭제")
    [SerializeField] private Text messageText;      // 메시지 (예: "정말로 삭제하시겠습니까?")
    [SerializeField] private Button confirmButton;  // 확인 버튼
    [SerializeField] private Button cancelButton;   // 취소 버튼

    // 콜백 함수들
    private Action onConfirm;
    private Action onCancel;

    void Start()
    {
        // 버튼 이벤트 연결
        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        // 초기 상태는 숨김
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 다이얼로그 열기
    /// </summary>
    /// <param name="title">다이얼로그 제목</param>
    /// <param name="message">확인 메시지</param>
    /// <param name="onConfirmCallback">확인 버튼 클릭 시 호출될 함수</param>
    /// <param name="onCancelCallback">취소 버튼 클릭 시 호출될 함수 (선택사항)</param>
    public void Show(string title, string message, Action onConfirmCallback, Action onCancelCallback = null)
    {
        // UI 텍스트 설정
        if (titleText != null)
            titleText.text = title;
        
        if (messageText != null)
            messageText.text = message;

        // 콜백 함수 저장
        onConfirm = onConfirmCallback;
        onCancel = onCancelCallback;

        // 다이얼로그 표시
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 다이얼로그 닫기
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        
        // 콜백 초기화
        onConfirm = null;
        onCancel = null;
    }

    /// <summary>
    /// 확인 버튼 클릭
    /// </summary>
    private void OnConfirmClicked()
    {
        onConfirm?.Invoke();
        Hide();
    }

    /// <summary>
    /// 취소 버튼 클릭
    /// </summary>
    private void OnCancelClicked()
    {
        onCancel?.Invoke();
        Hide();
    }
}