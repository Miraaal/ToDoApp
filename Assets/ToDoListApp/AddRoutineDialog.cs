using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 루틴 추가 다이얼로그
/// 간단한 입력 폼으로 새 루틴을 추가합니다.
/// </summary>
public class AddRoutineDialog : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private InputField titleInput;
    [SerializeField] private Dropdown typeDropdown;
    [SerializeField] private Dropdown categoryDropdown;
    [SerializeField] private InputField descriptionInput;

    [Header("Buttons")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private RoutineUIController controller;

    /// <summary>
    /// 다이얼로그 초기화
    /// </summary>
    public void Initialize(RoutineUIController ctrl)
    {
        controller = ctrl;

        // 버튼 이벤트 연결
        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        // 타입 드롭다운 초기화
        typeDropdown.ClearOptions();
        typeDropdown.AddOptions(new System.Collections.Generic.List<string> { "매일", "매주", "매월" });
        typeDropdown.value = 0;

        // 카테고리 드롭다운 초기화
        categoryDropdown.ClearOptions();
        categoryDropdown.AddOptions(new System.Collections.Generic.List<string> { "운동", "공부", "건강", "취미", "기타" });
        categoryDropdown.value = 0;

        // 초기 상태는 숨김
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 다이얼로그 열기
    /// </summary>
    public void Open()
    {
        // 입력 필드 초기화
        titleInput.text = "";
        descriptionInput.text = "";
        typeDropdown.value = 0;
        categoryDropdown.value = 0;

        gameObject.SetActive(true);
        titleInput.ActivateInputField(); // 제목 입력에 포커스
    }

    /// <summary>
    /// 다이얼로그 닫기
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 확인 버튼 클릭
    /// </summary>
    private void OnConfirmClicked()
    {
        // 유효성 검사
        string title = titleInput.text.Trim();
        if (string.IsNullOrEmpty(title))
        {
            Debug.LogWarning("⚠️ 루틴 이름을 입력하세요.");
            return;
        }

        // 타입 변환 (매일 → daily, 매주 → weekly, 매월 → monthly)
        string type = GetTypeFromDropdown(typeDropdown.value);
        string category = categoryDropdown.options[categoryDropdown.value].text;
        string description = descriptionInput.text.Trim();

        // 컨트롤러에 추가 요청
        controller.OnAddRoutineRequested(title, type, category, description);

        // 다이얼로그 닫기
        Close();
    }

    /// <summary>
    /// 취소 버튼 클릭
    /// </summary>
    private void OnCancelClicked()
    {
        Close();
    }

    /// <summary>
    /// 드롭다운 인덱스를 영문 타입으로 변환
    /// </summary>
    private string GetTypeFromDropdown(int index)
    {
        switch (index)
        {
            case 0: return "daily";
            case 1: return "weekly";
            case 2: return "monthly";
            default: return "daily";
        }
    }
}
