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

    //[Header("루틴 목록 UI (향후 추가 예정)")]
    // TODO: 루틴 목록을 표시할 UI 요소들
    // [SerializeField] private Transform routineListContainer;
    // [SerializeField] private GameObject routinePrefab;

    async void Start()
    {
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
        RoutineData[] routines = await DatabaseManager.Instance.GetActiveRoutinesAsync();

        // TODO: 실제 UI에 루틴 목록 표시하는 로직 추가
        // 현재는 콘솔에 출력만 수행
        foreach (var routine in routines)
        {
            Debug.Log($"루틴 #{routine.id}: {routine.title} ({routine.type}, {routine.category})");
        }

        Debug.Log($"📊 총 {routines.Length}개의 루틴을 표시했습니다.");
    }

    /// <summary>
    /// 새 루틴 추가 (UI 버튼에서 호출 가능)
    /// </summary>
    public async void OnAddRoutineButtonClicked()
    {
        // TODO: 실제 UI에서 입력받은 값 사용
        string title = "새 루틴";
        string type = "daily";
        string category = "일반";

        ShowLoading("루틴 추가 중...");

        int newId = await DatabaseManager.Instance.AddRoutineAsync(title, type, category);

        if (newId > 0)
        {
            Debug.Log($"✅ 루틴 추가 성공! ID: {newId}");
            await LoadRoutinesAsync(); // 목록 새로고침
        }

        HideLoading();
    }

    /// <summary>
    /// 루틴 완료 상태 토글 (UI 체크박스에서 호출 가능)
    /// </summary>
    public async void OnRoutineToggled(int routineId)
    {
        // 현재 날짜 가져오기 (type에 따라 조정 필요)
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        bool success = await DatabaseManager.Instance.ToggleRoutineCompletionAsync(routineId, today);

        if (success)
        {
            Debug.Log($"✅ 루틴 {routineId} 완료 상태 변경");
            // TODO: UI 업데이트 (체크박스 상태 변경 등)
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
