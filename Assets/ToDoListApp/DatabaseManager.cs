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
        command.CommandText = "SELECT id, title, type, category, description, created_at FROM routines WHERE is_active = 1";

        var reader = command.ExecuteReader();
        var routines = new System.Collections.Generic.List<RoutineData>();

        while (reader.Read())
        {
            var routine = new RoutineData
            {
                id = Convert.ToInt32(reader.GetValue(0)),
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
                INSERT INTO routines (title, type, category, description, is_active, created_at, updated_at)
                VALUES (@title, @type, @category, @description, 1, datetime('now', '+9 hours'), datetime('now', '+9 hours'));
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
    /// 루틴 완료 상태를 특정 값으로 설정
    /// </summary>
    public async UniTask<bool> SetRoutineCompletionAsync(int routineId, string startDate, bool isCompleted)
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
                SELECT id FROM routine_completions
                WHERE routine_id = @routineId AND start_date = @startDate";

            AddParameter(checkCommand, "@routineId", routineId);
            AddParameter(checkCommand, "@startDate", startDate);

            var reader = checkCommand.ExecuteReader();
            bool exists = reader.Read();
            int completionId = -1;

            if (exists)
            {
                // SQLite에서 id를 안전하게 읽기 (TEXT나 INTEGER 모두 처리)
                var idValue = reader.GetValue(0);
                completionId = Convert.ToInt32(idValue);
            }

            reader.Close();

            if (exists)
            {
                // 기존 기록 업데이트
                var command = dbConnection.CreateCommand();
                command.CommandText = @"
                    UPDATE routine_completions
                    SET is_completed = @isCompleted,
                        completed_at = CASE WHEN @isCompleted = 1 THEN datetime('now', '+9 hours') ELSE NULL END,
                        updated_at = datetime('now', '+9 hours')
                    WHERE id = @id";

                AddParameter(command, "@id", completionId);
                AddParameter(command, "@isCompleted", isCompleted ? 1 : 0);

                command.ExecuteNonQuery();
            }
            else if (isCompleted)
            {
                // 새 완료 기록 추가 (체크된 경우만)
                var command = dbConnection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO routine_completions (id, routine_id, start_date, is_completed, completed_at, updated_at)
                    VALUES (NULL, @routineId, @startDate, 1, datetime('now', '+9 hours'), datetime('now', '+9 hours'))";

                AddParameter(command, "@routineId", routineId);
                AddParameter(command, "@startDate", startDate);

                command.ExecuteNonQuery();
            }

            await UniTask.SwitchToMainThread();
            return true;
        }
        catch (Exception ex)
        {
            await UniTask.SwitchToMainThread();
            Debug.LogError($"❌ 완료 상태 변경 실패: {ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }


    /// <summary>
    /// 특정 날짜의 모든 루틴 완료 상태를 한번에 조회
    /// </summary>
    public async UniTask<System.Collections.Generic.Dictionary<int, bool>> GetAllRoutineCompletionStatusAsync(string startDate)
    {
        var completionStatus = new System.Collections.Generic.Dictionary<int, bool>();

        if (!IsConnected)
        {
            Debug.LogError("❌ DB가 연결되지 않았습니다.");
            return completionStatus;
        }

        await UniTask.SwitchToThreadPool();

        try
        {
            var command = dbConnection.CreateCommand();
            command.CommandText = @"
                SELECT routine_id, is_completed FROM routine_completions
                WHERE start_date = @startDate";

            AddParameter(command, "@startDate", startDate);

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int routineId = Convert.ToInt32(reader.GetValue(0));
                // SQLite에서 0/1을 안전하게 읽기
                var statusValue = reader.IsDBNull(1) ? 0 : reader.GetValue(1);
                bool isCompleted = Convert.ToInt32(statusValue) != 0;
                completionStatus[routineId] = isCompleted;
            }

            reader.Close();

            await UniTask.SwitchToMainThread();

            return completionStatus;
        }
        catch (Exception ex)
        {
            await UniTask.SwitchToMainThread();
            Debug.LogError($"❌ 완료 상태 조회 실패: {ex.Message}\n{ex.StackTrace}");
            return completionStatus;
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

    /// <summary>
    /// 루틴 삭제 (관련된 완료 기록도 함께 삭제)
    /// </summary>
    public async UniTask<bool> DeleteRoutineAsync(int routineId)
    {
        if (!IsConnected)
        {
            Debug.LogError("❌ DB가 연결되지 않았습니다.");
            return false;
        }

        await UniTask.SwitchToThreadPool();

        try
        {
            // 트랜잭션 시작
            var transaction = dbConnection.BeginTransaction();

            try
            {
                // 1. 관련된 완료 기록 삭제
                var deleteCompletionsCommand = dbConnection.CreateCommand();
                deleteCompletionsCommand.Transaction = transaction;
                deleteCompletionsCommand.CommandText = "DELETE FROM routine_completions WHERE routine_id = @routineId";
                AddParameter(deleteCompletionsCommand, "@routineId", routineId);
                int completionsDeleted = deleteCompletionsCommand.ExecuteNonQuery();

                // 2. 루틴 삭제
                var deleteRoutineCommand = dbConnection.CreateCommand();
                deleteRoutineCommand.Transaction = transaction;
                deleteRoutineCommand.CommandText = "DELETE FROM routines WHERE id = @routineId";
                AddParameter(deleteRoutineCommand, "@routineId", routineId);
                int routinesDeleted = deleteRoutineCommand.ExecuteNonQuery();

                // 트랜잭션 커밋
                transaction.Commit();

                await UniTask.SwitchToMainThread();
                
                if (routinesDeleted > 0)
                {
                    Debug.Log($"✅ 루틴 삭제 완료: ID {routineId} (관련 완료기록 {completionsDeleted}개도 삭제됨)");
                    return true;
                }
                else
                {
                    Debug.LogWarning($"⚠️ 삭제할 루틴을 찾을 수 없습니다: ID {routineId}");
                    return false;
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            await UniTask.SwitchToMainThread();
            Debug.LogError($"❌ 루틴 삭제 실패: {ex.Message}");
            return false;
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
