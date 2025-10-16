using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// 데이터베이스 연결 및 쿼리 관리를 담당하는 싱글톤 클래스
/// 앱 전체에서 하나의 DB 연결을 공유하여 사용
/// </summary>
public class DatabaseManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static DatabaseManager _instance;
    public static DatabaseManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("❌ DatabaseManager가 초기화되지 않았습니다!");
            }
            return _instance;
        }
    }

    // DB 연결 객체
    private IDbConnection dbConnection;

    // DB 연결 상태
    public bool IsConnected { get; private set; } = false;

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
    }

    /// <summary>
    /// 데이터베이스 연결을 비동기로 수행
    /// </summary>
    public async UniTask<bool> ConnectDatabaseAsync()
    {
        try
        {
            // 백그라운드 스레드로 전환
            await UniTask.SwitchToThreadPool();

            // DB 파일 경로 설정
            string dbname = "ToDoList_DB";
            string connectionString = "URI=file:" + Application.streamingAssetsPath + "/" + dbname + ".db";

            // DB 연결 생성 및 열기
            dbConnection = new SqliteConnection(connectionString);
            dbConnection.Open();

            // 연결 테스트: DB 버전 확인
            var command = dbConnection.CreateCommand();
            command.CommandText = "SELECT sqlite_version()";
            var version = command.ExecuteScalar();

            // 메인 스레드로 복귀
            await UniTask.SwitchToMainThread();

            IsConnected = true;
            Debug.Log($"✅ 데이터베이스 연결 성공! SQLite 버전: {version}");
            return true;
        }
        catch (Exception ex)
        {
            await UniTask.SwitchToMainThread();
            IsConnected = false;
            Debug.LogError($"❌ 데이터베이스 연결 실패: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 활성화된 루틴 목록을 조회
    /// </summary>
    public async UniTask<RoutineData[]> GetActiveRoutinesAsync()
    {
        if (!IsConnected)
        {
            Debug.LogError("❌ DB가 연결되지 않았습니다.");
            return new RoutineData[0];
        }

        await UniTask.SwitchToThreadPool();

        var command = dbConnection.CreateCommand();
        command.CommandText = "SELECT id, title, type, category, description, created_at FROM Routines WHERE is_active = 1";

        var reader = command.ExecuteReader();
        var routines = new System.Collections.Generic.List<RoutineData>();

        while (reader.Read())
        {
            var routine = new RoutineData
            {
                id = reader.GetInt32(0),
                title = reader.GetString(1),
                type = reader.GetString(2),
                category = reader.IsDBNull(3) ? "" : reader.GetString(3),
                description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                createdAt = reader.IsDBNull(5) ? "" : reader.GetString(5)
            };

            routines.Add(routine);
        }

        reader.Close();

        await UniTask.SwitchToMainThread();

        Debug.Log($"📊 총 {routines.Count}개의 활성 루틴을 불러왔습니다.");
        return routines.ToArray();
    }

    /// <summary>
    /// 새로운 루틴 추가
    /// </summary>
    public async UniTask<int> AddRoutineAsync(string title, string type, string category = "", string description = "")
    {
        if (!IsConnected)
        {
            Debug.LogError("❌ DB가 연결되지 않았습니다.");
            return -1;
        }

        await UniTask.SwitchToThreadPool();

        try
        {
            var command = dbConnection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Routines (title, type, category, description, is_active, created_at, updated_at)
                VALUES (@title, @type, @category, @description, 1, datetime('now'), datetime('now'));
                SELECT last_insert_rowid();";

            AddParameter(command, "@title", title);
            AddParameter(command, "@type", type);
            AddParameter(command, "@category", category);
            AddParameter(command, "@description", description);

            var newId = Convert.ToInt32(command.ExecuteScalar());

            await UniTask.SwitchToMainThread();
            Debug.Log($"✅ 루틴 추가 완료: {title} (ID: {newId})");
            return newId;
        }
        catch (Exception ex)
        {
            await UniTask.SwitchToMainThread();
            Debug.LogError($"❌ 루틴 추가 실패: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// 루틴 완료 상태 토글
    /// </summary>
    public async UniTask<bool> ToggleRoutineCompletionAsync(int routineId, string startDate)
    {
        if (!IsConnected)
        {
            Debug.LogError("❌ DB가 연결되지 않았습니다.");
            return false;
        }

        await UniTask.SwitchToThreadPool();

        try
        {
            // 기존 완료 기록 확인
            var checkCommand = dbConnection.CreateCommand();
            checkCommand.CommandText = @"
                SELECT id, is_completed FROM RoutineCompletions
                WHERE routine_id = @routineId AND start_date = @startDate";

            AddParameter(checkCommand, "@routineId", routineId);
            AddParameter(checkCommand, "@startDate", startDate);

            var reader = checkCommand.ExecuteReader();
            bool exists = reader.Read();
            int completionId = exists ? reader.GetInt32(0) : -1;
            bool currentStatus = exists && reader.GetBoolean(1);
            reader.Close();

            IDbCommand command;

            if (exists)
            {
                // 기존 기록 업데이트
                command = dbConnection.CreateCommand();
                command.CommandText = @"
                    UPDATE RoutineCompletions
                    SET is_completed = @newStatus,
                        completed_at = CASE WHEN @newStatus = 1 THEN datetime('now') ELSE NULL END,
                        updated_at = datetime('now')
                    WHERE id = @id";

                AddParameter(command, "@id", completionId);
                AddParameter(command, "@newStatus", !currentStatus);
            }
            else
            {
                // 새 완료 기록 추가
                command = dbConnection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO RoutineCompletions (routine_id, start_date, is_completed, completed_at, updated_at)
                    VALUES (@routineId, @startDate, 1, datetime('now'), datetime('now'))";

                AddParameter(command, "@routineId", routineId);
                AddParameter(command, "@startDate", startDate);
            }

            command.ExecuteNonQuery();

            await UniTask.SwitchToMainThread();
            Debug.Log($"✅ 루틴 완료 상태 변경: {routineId} ({startDate})");
            return true;
        }
        catch (Exception ex)
        {
            await UniTask.SwitchToMainThread();
            Debug.LogError($"❌ 완료 상태 변경 실패: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// SQL 파라미터 추가 헬퍼 메서드
    /// </summary>
    private void AddParameter(IDbCommand command, string parameterName, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    /// <summary>
    /// 앱 종료 시 DB 연결 해제
    /// </summary>
    void OnDestroy()
    {
        if (dbConnection != null)
        {
            dbConnection.Close();
            dbConnection.Dispose();
            IsConnected = false;
            Debug.Log("🔒 데이터베이스 연결 종료");
        }
    }
}

/// <summary>
/// 루틴 데이터 구조체
/// </summary>
[System.Serializable]
public struct RoutineData
{
    public int id;
    public string title;
    public string type;
    public string category;
    public string description;
    public string createdAt;
}
