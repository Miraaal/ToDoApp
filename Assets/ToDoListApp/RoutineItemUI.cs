using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class RoutineItemUI : MonoBehaviour
{
    // Inspector에서 연결할 UI 요소들
    [SerializeField] private Text titleText;
    [SerializeField] private Text categoryText;
    [SerializeField] private Toggle completionToggle;  // 체크박스
    [SerializeField] private Text typeText; // (옵션)

    private RoutineData routineData;
    private RoutineUIController controller;

    /// <summary>
    /// 루틴 데이터로 UI 초기화 및 컨트롤러 참조 저장
    /// </summary>
    public void Initialize(RoutineData data, RoutineUIController ctrl, bool isCompleted)
    {
        routineData = data;
        controller = ctrl;

        // UI에 데이터 표시
        titleText.text = data.title;
        categoryText.text = data.category;

        // 완료 상태 설정 (이벤트 발생 방지)
        completionToggle.SetIsOnWithoutNotify(isCompleted);

        // Toggle 이벤트 연결
        completionToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    /// <summary>
    /// 체크박스 클릭 시 호출되는 메소드
    /// </summary>
    private async void OnToggleChanged(bool isChecked)
    {
        // 컨트롤러에게 완료 상태 변경 요청 (새로운 상태를 전달)
        bool success = await controller.OnRoutineToggledAsync(routineData.id, routineData.type, isChecked);

        // DB 업데이트 실패 시 UI를 원래 상태로 롤백
        if (!success)
        {
            completionToggle.SetIsOnWithoutNotify(!isChecked);
        }
    }
}