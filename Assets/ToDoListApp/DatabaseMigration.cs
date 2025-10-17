using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;

/// <summary>
/// 데이터베이스 스키마 마이그레이션 스크립트
/// Unity 에디터에서 실행하여 테이블을 올바른 스키마로 재생성합니다.
///
/// 사용법:
/// 1. 이 스크립트를 씬의 GameObject에 추가
/// 2. Unity 에디터에서 Play 모드 실행
/// 3. 콘솔에서 마이그레이션 결과 확인
/// 4. 완료 후 이 컴포넌트 제거
/// </summary>
public class DatabaseMigration : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔄 데이터베이스 마이그레이션 시작...");
        MigrateDatabase();
    }

    void MigrateDatabase()
    {
        string dbname = "ToDoList_DB";
        string connectionString = "URI=file:" + Application.streamingAssetsPath + "/" + dbname + ".db";
        IDbConnection dbConnection = null;

        try
        {
            // DB 연결
            dbConnection = new SqliteConnection(connectionString);
            dbConnection.Open();
            Debug.Log("✅ 데이터베이스 연결 성공");

            // 1. 기존 테이블 존재 확인
            bool routinesExists = TableExists(dbConnection, "routines");
            bool completionsExists = TableExists(dbConnection, "routine_completions");
            Debug.Log($"🔍 기존 테이블 확인: routines={routinesExists}, routine_completions={completionsExists}");

            // 2. 백업 테이블 생성 (테이블이 존재하는 경우만)
            if (routinesExists || completionsExists)
            {
                Debug.Log("📦 기존 데이터 백업 중...");
                ExecuteNonQuery(dbConnection, "DROP TABLE IF EXISTS routines_backup");
                ExecuteNonQuery(dbConnection, "DROP TABLE IF EXISTS routine_completions_backup");

                if (routinesExists)
                {
                    ExecuteNonQuery(dbConnection, "CREATE TABLE routines_backup AS SELECT * FROM routines");
                    Debug.Log("✅ routines 백업 완료");
                }

                if (completionsExists)
                {
                    ExecuteNonQuery(dbConnection, "CREATE TABLE routine_completions_backup AS SELECT * FROM routine_completions");
                    Debug.Log("✅ routine_completions 백업 완료");
                }
            }
            else
            {
                Debug.Log("ℹ️ 기존 테이블이 없습니다. 새로 생성합니다.");
            }

            // 3. 기존 테이블 삭제
            Debug.Log("🗑️ 기존 테이블 삭제 중...");
            ExecuteNonQuery(dbConnection, "DROP TABLE IF EXISTS routines");
            ExecuteNonQuery(dbConnection, "DROP TABLE IF EXISTS routine_completions");
            Debug.Log("✅ 기존 테이블 삭제 완료");

            // 3. 새 테이블 생성 (올바른 스키마)
            Debug.Log("🏗️ 새 테이블 생성 중...");

            string createRoutines = @"
                CREATE TABLE routines (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    title TEXT NOT NULL,
                    type TEXT NOT NULL,
                    category TEXT,
                    description TEXT,
                    is_active INTEGER DEFAULT 1,
                    created_at TEXT DEFAULT (datetime('now', '+9 hours')),
                    updated_at TEXT DEFAULT (datetime('now', '+9 hours'))
                )";
            ExecuteNonQuery(dbConnection, createRoutines);

            string createCompletions = @"
                CREATE TABLE routine_completions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    routine_id INTEGER NOT NULL,
                    start_date TEXT NOT NULL,
                    is_completed INTEGER DEFAULT 0,
                    completed_at TEXT,
                    updated_at TEXT DEFAULT (datetime('now', '+9 hours')),
                    FOREIGN KEY (routine_id) REFERENCES routines(id)
                )";
            ExecuteNonQuery(dbConnection, createCompletions);
            Debug.Log("✅ 새 테이블 생성 완료");

            // 4. 데이터 복원 (백업이 존재하는 경우만)
            if (routinesExists || completionsExists)
            {
                Debug.Log("📥 데이터 복원 중...");

                if (routinesExists)
                {
                    string restoreRoutines = @"
                        INSERT INTO routines (id, title, type, category, description, is_active, created_at, updated_at)
                        SELECT
                            CAST(id AS INTEGER),
                            title,
                            type,
                            category,
                            description,
                            CAST(is_active AS INTEGER),
                            created_at,
                            updated_at
                        FROM routines_backup";
                    int routinesCount = ExecuteNonQuery(dbConnection, restoreRoutines);
                    Debug.Log($"✅ routines 테이블: {routinesCount}개 행 복원");
                }

                if (completionsExists)
                {
                    string restoreCompletions = @"
                        INSERT INTO routine_completions (id, routine_id, start_date, is_completed, completed_at, updated_at)
                        SELECT
                            CAST(id AS INTEGER),
                            CAST(routine_id AS INTEGER),
                            start_date,
                            CAST(is_completed AS INTEGER),
                            completed_at,
                            updated_at
                        FROM routine_completions_backup
                        WHERE id IS NOT NULL AND id != ''";
                    int completionsCount = ExecuteNonQuery(dbConnection, restoreCompletions);
                    Debug.Log($"✅ routine_completions 테이블: {completionsCount}개 행 복원");
                }
            }
            else
            {
                Debug.Log("ℹ️ 복원할 데이터가 없습니다. CSV 파일에서 데이터를 가져오세요.");
            }

            // 5. 스키마 확인
            Debug.Log("🔍 새 스키마 확인...");
            var command = dbConnection.CreateCommand();
            command.CommandText = "SELECT sql FROM sqlite_master WHERE type='table' AND name='routine_completions'";
            var schema = command.ExecuteScalar();
            Debug.Log($"📐 routine_completions 스키마:\n{schema}");

            // 6. 백업 테이블 삭제 (선택사항)
            // ExecuteNonQuery(dbConnection, "DROP TABLE routines_backup");
            // ExecuteNonQuery(dbConnection, "DROP TABLE routine_completions_backup");
            Debug.Log("ℹ️ 백업 테이블은 안전을 위해 보관됩니다 (routines_backup, routine_completions_backup)");

            Debug.Log("🎉 데이터베이스 마이그레이션 완료! 이제 이 컴포넌트를 제거해도 됩니다.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ 마이그레이션 실패: {ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
    }

    int ExecuteNonQuery(IDbConnection connection, string sql)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;
        return command.ExecuteNonQuery();
    }

    bool TableExists(IDbConnection connection, string tableName)
    {
        var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@tableName";
        parameter.Value = tableName;
        command.Parameters.Add(parameter);

        var result = command.ExecuteScalar();
        return result != null;
    }
}
