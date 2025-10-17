# ToDoApp

> Unity 기반 일상 루틴 관리 애플리케이션

## 프로젝트 개요

일일/주간/월간 루틴을 관리하고 완료 여부를 추적할 수 있는 생산성 앱입니다. SQLite 데이터베이스를 활용하여 사용자의 루틴 데이터를 영구 저장하며, 직관적인 스와이프 UI를 통해 편리한 사용 경험을 제공합니다.

## 기술 스택

### 개발 환경
- **Unity 버전**: 6000.0.59f2 (Unity 6)
- **렌더 파이프라인**: Universal Render Pipeline (URP) 17.0.4
- **플랫폼**: Windows, Android

### 주요 라이브러리
- **UniTask** (Cysharp) - 비동기 작업 처리
- **SQLite** (Mono.Data.Sqlite) - 로컬 데이터베이스
- **Unity Input System** 1.14.2 - 입력 처리
- **Unity UI (UGUI)** 2.0.0 - 사용자 인터페이스

## 데이터베이스 구조

### routines 테이블
```sql
CREATE TABLE routines (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL,                    -- 루틴 제목
    type TEXT NOT NULL,                     -- 'daily', 'weekly', 'monthly'
    category TEXT,                          -- 카테고리 (운동, 공부, 건강, 취미 등)
    description TEXT,                       -- 설명
    is_active INTEGER DEFAULT 1,            -- 활성 상태 (0/1)
    created_at TEXT,                        -- 생성 일시
    updated_at TEXT                         -- 수정 일시
);
```

### routine_completions 테이블
```sql
CREATE TABLE routine_completions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    routine_id INTEGER NOT NULL,            -- routines.id 참조
    start_date TEXT NOT NULL,               -- 기준 날짜 (일일/주간/월간에 따라 다름)
    is_completed INTEGER DEFAULT 0,         -- 완료 여부 (0/1)
    completed_at TEXT,                      -- 완료 시간
    updated_at TEXT,                        -- 수정 일시
    FOREIGN KEY (routine_id) REFERENCES routines(id)
);
```

## 프로젝트 구조

```
Assets/
├── ToDoListApp/
│   ├── DatabaseManager.cs          # 싱글톤 DB 연결 및 쿼리 관리
│   ├── RoutineUIController.cs      # UI 이벤트 처리 및 화면 업데이트
│   └── SwipeUI.cs                  # 스와이프 페이지 네비게이션
├── Scenes/
│   ├── SampleScene.unity            # 샘플/테스트 씬
│   └── TodoLIst.unity               # 메인 애플리케이션 씬
└── StreamingAssets/
    └── ToDoList_DB.db               # SQLite 데이터베이스 (런타임 접근)
```

## 주요 기능 (2025-10-16 기준)

### ✅ 구현 완료
- **데이터베이스 연결 관리**
  - 싱글톤 패턴 기반 DatabaseManager
  - 비동기 DB 연결 (UniTask)
  - 연결 상태 체크 및 에러 처리

- **루틴 관리 기능**
  - 활성 루틴 목록 조회 (`GetActiveRoutinesAsync`)
  - 새 루틴 추가 (`AddRoutineAsync`)
  - 루틴 완료 상태 설정 (`SetRoutineCompletionAsync`)
  - 날짜별 완료 상태 일괄 조회 (`GetAllRoutineCompletionStatusAsync`)
  - SQL 파라미터 바인딩으로 SQL Injection 방지

- **UI 컨트롤러**
  - 로딩 화면 표시 및 관리
  - 비동기 초기화 프로세스
  - 에러 핸들링 및 사용자 피드백

- **스와이프 UI**
  - 좌우 스와이프 페이지 네비게이션
  - 탭 인디케이터 자동 업데이트
  - 에디터(마우스) 및 Android(터치) 입력 모두 지원

### 🚧 진행 중 / 예정
- [ ] 루틴 목록 UI 구현
- [ ] 루틴 추가/수정 다이얼로그
- [ ] 완료 통계 및 시각화
- [ ] 캘린더 뷰 통합
- [ ] 알림 기능

## 개발 가이드

### 프로젝트 열기
1. Unity Hub 실행
2. 프로젝트 추가: 이 디렉토리 선택
3. Unity 6000.0.59f2 버전 사용
4. 프로젝트가 열리면 `TodoLIst.unity` 씬 로드

### 데이터베이스 초기화

**중요**: CSV import는 PRIMARY KEY가 제대로 설정되지 않으므로 사용하지 마세요!

**권장 방법**: Unity 에디터에서 마이그레이션 스크립트 실행
1. 빈 GameObject 생성
2. `DatabaseMigration.cs` 컴포넌트 추가
3. Play 모드로 실행
4. 콘솔에서 완료 확인 후 GameObject 삭제

마이그레이션은 올바른 스키마로 테이블을 생성하고 기존 데이터가 있으면 백업 후 복원합니다.

### 빌드 주의사항
- **Windows**: StreamingAssets의 DB 파일이 읽기 전용
- **Android**: APK에 포함된 DB는 수정 불가하므로 앱 시작 시 내부 저장소로 복사 필요

## 코드 구조

### DatabaseManager.cs
- 싱글톤 패턴으로 전역 DB 접근 제공
- UniTask 기반 비동기 쿼리
- 씬 전환 시에도 인스턴스 유지 (`DontDestroyOnLoad`)

### RoutineUIController.cs
- 앱 초기화 프로세스 관리
- DB와 UI 연결 역할
- 로딩 상태 표시 및 에러 처리

### SwipeUI.cs
- Scrollbar 기반 페이지 전환
- 스와이프 제스처 감지 및 처리
- 부드러운 애니메이션 (Lerp)

## 라이선스

개인 프로젝트 (비상업적 용도)

---

**마지막 업데이트**: 2025-10-17
