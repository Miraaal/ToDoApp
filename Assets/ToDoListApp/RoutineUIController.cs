using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// UI 이벤트 처리 및 화면 업데이트를 담당하는 컨트롤러
/// DatabaseManager를 통해 데이터를 조회하고 UI에 표시
/// </summary>
public class RoutineUIController : MonoBehaviour
{
    [Header("로딩 UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Text loadingText;

    [Header("루틴 목록 UI")]
    [SerializeField] private Transform routineListContainer;
    [SerializeField] private GameObject routinePrefab;

    [Header("루틴 추가 UI")]
    [SerializeField] private Button addRoutineButton;
    [SerializeField] private AddRoutineDialog addRoutineDialog;

    [Header("확인 다이얼로그")]
    [SerializeField] private ConfirmationDialog confirmationDialog;

    async void Start()
    {
        // 다이얼로그 초기화
        if (addRoutineDialog != null)
        {
            addRoutineDialog.Initialize(this);
        }

        // 버튼 이벤트 연결
        if (addRoutineButton != null)
        {
            addRoutineButton.onClick.AddListener(OnAddRoutineButtonClicked);
        }

        // 로딩 화면 표시
        ShowLoading("초기화 중...");

        try
        {
            // 1단계: DB 연결
            UpdateLoadingText("데이터베이스 연결 중...");
            bool connected = await DatabaseManager.Instance.ConnectDatabaseAsync();

            if (!connected)
            {
                UpdateLoadingText("데이터베이스 연결 실패");
                await UniTask.Delay(2000);
                HideLoading();
                return;
            }

            // 2단계: 초기 데이터 로딩
            UpdateLoadingText("데이터 불러오는 중...");
            await LoadRoutinesAsync();

            // 3단계: 로딩 완료
            UpdateLoadingText("완료!");
            await UniTask.Delay(500);

            HideLoading();
            Debug.Log("🎉 초기화 완료! 앱 사용 가능");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ 초기화 오류: {ex.Message}");
            UpdateLoadingText($"오류: {ex.Message}");
            await UniTask.Delay(2000);
            HideLoading();
        }
    }

    /// <summary>
    /// 루틴 목록을 불러와서 UI에 표시
    /// </summary>
    private async UniTask LoadRoutinesAsync()
    {
        // 1. 기존 UI 아이템 제거
        foreach (Transform child in routineListContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. DB에서 루틴 목록 가져오기
        RoutineData[] routines = await DatabaseManager.Instance.GetActiveRoutinesAsync();

        // 3. 오늘 날짜의 완료 상태 일괄 조회
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        var completionStatus = await DatabaseManager.Instance.GetAllRoutineCompletionStatusAsync(today);

        // 4. 각 루틴마다 프리팹 인스턴스 생성
        foreach (var routine in routines)
        {
            GameObject itemObj = Instantiate(routinePrefab, routineListContainer);
            RoutineItemUI itemUI = itemObj.GetComponent<RoutineItemUI>();

            // 완료 상태 확인 (Dictionary에서 조회)
            bool isCompleted = completionStatus.ContainsKey(routine.id) ? completionStatus[routine.id] : false;

            itemUI.Initialize(routine, this, isCompleted);
        }

        Debug.Log($"📊 총 {routines.Length}개의 루틴을 표시했습니다.");
    }

    /// <summary>
    /// 루틴 추가 버튼 클릭 (+ 버튼)
    /// </summary>
    public void OnAddRoutineButtonClicked()
    {
        if (addRoutineDialog != null)
        {
            addRoutineDialog.Open();
        }
        else
        {
            Debug.LogError("❌ AddRoutineDialog가 연결되지 않았습니다!");
        }
    }

    /// <summary>
    /// 다이얼로그에서 루틴 추가 요청 (AddRoutineDialog에서 호출)
    /// </summary>
    public async void OnAddRoutineRequested(string title, string type, string category, string description)
    {
        ShowLoading("루틴 추가 중...");

        int newId = await DatabaseManager.Instance.AddRoutineAsync(title, type, category, description);

        if (newId > 0)
        {
            Debug.Log($"✅ 루틴 추가 성공! ID: {newId}, 제목: {title}");
            await LoadRoutinesAsync(); // 목록 새로고침
        }
        else
        {
            Debug.LogError("❌ 루틴 추가 실패");
        }

        HideLoading();
    }

    /// <summary>
    /// 루틴 완료 상태 설정 (UI 체크박스에서 호출)
    /// </summary>
    public async UniTask<bool> OnRoutineToggledAsync(int routineId, string routineType, bool isCompleted)
    {
        // 루틴 타입에 맞는 시작 날짜 계산
        string startDate = GetStartDateForType(routineType);

        bool success = await DatabaseManager.Instance.SetRoutineCompletionAsync(routineId, startDate, isCompleted);

        if (success)
        {
            Debug.Log($"✅ 루틴 {routineId} 완료 상태 변경: {isCompleted} ({startDate})");
        }
        else
        {
            Debug.LogError($"❌ 루틴 {routineId} 완료 상태 변경 실패");
        }

        return success;
    }

    /// <summary>
    /// 루틴 삭제 요청 (RoutineItemUI에서 호출)
    /// </summary>
    public async UniTask OnDeleteRoutineRequestedAsync(int routineId, string routineTitle)
    {
        // 확인 다이얼로그가 연결되어 있으면 확인 팝업 표시
        if (confirmationDialog != null)
        {
            confirmationDialog.Show(
                "루틴 삭제",
                $"'{routineTitle}' 루틴을 정말로 삭제하시겠습니까?\n\n삭제된 루틴과 관련된 모든 완료 기록도 함께 삭제됩니다.",
                async () => await PerformDeleteRoutineAsync(routineId, routineTitle) // 확인 시 실행
            );
        }
        else
        {
            // 확인 다이얼로그가 없으면 바로 삭제 (기존 방식)
            await PerformDeleteRoutineAsync(routineId, routineTitle);
        }
    }

    /// <summary>
    /// 실제 루틴 삭제 수행
    /// </summary>
    private async UniTask PerformDeleteRoutineAsync(int routineId, string routineTitle)
    {
        ShowLoading("루틴 삭제 중...");

        bool success = await DatabaseManager.Instance.DeleteRoutineAsync(routineId);

        if (success)
        {
            Debug.Log($"✅ 루틴 삭제 성공: {routineTitle}");
            await LoadRoutinesAsync(); // 목록 새로고침
        }
        else
        {
            Debug.LogError($"❌ 루틴 삭제 실패: {routineTitle}");
        }

        HideLoading();
    }

    /// <summary>
    /// 루틴 타입에 맞는 시작 날짜 계산
    /// </summary>
    private string GetStartDateForType(string type)
    {
        DateTime now = DateTime.Now;

        switch (type.ToLower())
        {
            case "weekly":
                // 이번 주 일요일
                int daysToSunday = (int)now.DayOfWeek;
                return now.AddDays(-daysToSunday).ToString("yyyy-MM-dd");

            case "monthly":
                // 이번 달 1일
                return new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");

            default: // "daily"
                return now.ToString("yyyy-MM-dd");
        }
    }

    #region 로딩 UI 관리

    private void ShowLoading(string message = "로딩 중...")
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        if (loadingText != null)
        {
            loadingText.text = message;
        }
    }

    private void HideLoading()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    private void UpdateLoadingText(string message)
    {
        if (loadingText != null)
        {
            loadingText.text = message;
        }
    }

    #endregion
}
