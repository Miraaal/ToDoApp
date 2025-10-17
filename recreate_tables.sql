-- ToDoList 앱 데이터베이스 테이블 재생성 스크립트
-- 사용법: sqlite3 Assets/StreamingAssets/ToDoList_DB.db < recreate_tables.sql

-- 기존 테이블 백업
CREATE TABLE IF NOT EXISTS routines_backup AS SELECT * FROM routines;
CREATE TABLE IF NOT EXISTS routine_completions_backup AS SELECT * FROM routine_completions;

-- 기존 테이블 삭제
DROP TABLE IF EXISTS routines;
DROP TABLE IF EXISTS routine_completions;

-- routines 테이블 생성 (올바른 스키마)
CREATE TABLE routines (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL,
    type TEXT NOT NULL,
    category TEXT,
    description TEXT,
    is_active INTEGER DEFAULT 1,
    created_at TEXT,
    updated_at TEXT
);

-- routine_completions 테이블 생성 (올바른 스키마)
CREATE TABLE routine_completions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    routine_id INTEGER NOT NULL,
    start_date TEXT NOT NULL,
    is_completed INTEGER DEFAULT 0,
    completed_at TEXT,
    updated_at TEXT,
    FOREIGN KEY (routine_id) REFERENCES routines(id)
);

-- 백업 데이터 복원
INSERT INTO routines (id, title, type, category, description, is_active, created_at, updated_at)
SELECT id, title, type, category, description, is_active, created_at, updated_at
FROM routines_backup;

INSERT INTO routine_completions (id, routine_id, start_date, is_completed, completed_at, updated_at)
SELECT id, routine_id, start_date, is_completed, completed_at, updated_at
FROM routine_completions_backup
WHERE id IS NOT NULL AND id != '';

-- 백업 테이블 삭제 (선택사항 - 안전을 위해 남겨둘 수도 있음)
-- DROP TABLE routines_backup;
-- DROP TABLE routine_completions_backup;

-- 결과 확인
SELECT 'routines 테이블 행 수:', COUNT(*) FROM routines;
SELECT 'routine_completions 테이블 행 수:', COUNT(*) FROM routine_completions;
