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

## 주요 기능 (2025-10-17 기준)

### ✅ 구현 완료

#### 데이터베이스
- **연결 관리**
  - 싱글톤 패턴 기반 DatabaseManager
  - 비동기 DB 연결 (UniTask)
  - 연결 상태 체크 및 에러 처리

- **스키마 관리**
  - PRIMARY KEY AUTOINCREMENT 적용
  - 한국 시간(KST, UTC+9) 기준 날짜 저장
  - 타입 안전성 보장 (TEXT/INTEGER 자동 변환)
  - DB 마이그레이션 스크립트 (`DatabaseMigration.cs`)

#### 루틴 관리
- **CRUD 기능**
  - ✅ 활성 루틴 목록 조회 (`GetActiveRoutinesAsync`)
  - ✅ 새 루틴 추가 (`AddRoutineAsync`)
  - ✅ 루틴 삭제 (`DeleteRoutineAsync`) - 트랜잭션으로 완료 기록과 함께 안전 삭제
  - ✅ 루틴 완료 상태 설정 (`SetRoutineCompletionAsync`)
  - ✅ 날짜별 완료 상태 일괄 조회 (`GetAllRoutineCompletionStatusAsync`)
  - ✅ SQL 파라미터 바인딩으로 SQL Injection 방지

- **완료 상태 관리**
  - ✅ 체크박스 UI와 DB 실시간 동기화
  - ✅ DB 업데이트 실패 시 UI 자동 롤백
  - ✅ 일일/주간/월간 타입별 날짜 계산
  - ✅ 자정 기준 자동 초기화 (한국 시간)

#### UI 구현
- **루틴 목록**
  - ✅ 루틴 아이템 프리팹 (`RoutineItemUI`)
  - ✅ 동적 목록 생성 및 새로고침
  - ✅ 체크박스 상태 표시 및 토글

- **루틴 추가**
  - ✅ 추가 다이얼로그 스크립트 (`AddRoutineDialog`)
  - ✅ 입력 유효성 검사
  - ✅ 타입/카테고리 선택 (Dropdown)
  - ✅ UI 구성 및 테스트 완료

- **루틴 삭제**
  - ✅ 삭제 버튼 UI (`RoutineItemUI`)
  - ✅ 확인 다이얼로그 (`ConfirmationDialog`)
  - ✅ 트랜잭션 기반 안전한 삭제

- **스와이프 네비게이션**
  - ✅ 좌우 스와이프 페이지 전환
  - ✅ 탭 인디케이터 자동 업데이트
  - ✅ 에디터(마우스) 및 Android(터치) 입력 지원

### 🚧 진행 중
- [ ] 루틴 편집 기능 (추가/삭제 완료, 수정 기능 필요)
- [ ] 완료 통계 및 시각화
- [ ] 캘린더 뷰 통합

### 📋 예정
- [ ] 루틴 카테고리 커스터마이징
- [ ] 알림 기능
- [ ] 데이터 백업/복원
- [ ] 다크 모드

## 개발 가이드

### 프로젝트 열기
1. Unity Hub 실행
2. 프로젝트 추가: 이 디렉토리 선택
3. Unity 6000.0.59f2 버전 사용
4. 프로젝트가 열리면 `TodoLIst.unity` 씬 로드

### 데이터베이스 초기화

**⚠️ 중요**: CSV import는 PRIMARY KEY가 제대로 설정되지 않으므로 사용하지 마세요!

**권장 방법**: Unity 에디터에서 마이그레이션 스크립트 실행

#### 초기 설정 또는 스키마 변경 시
1. Unity 에디터에서 빈 GameObject 생성
2. `DatabaseMigration.cs` 컴포넌트 추가
3. Play 모드로 실행
4. 콘솔에서 완료 메시지 확인:
   ```
   🎉 데이터베이스 마이그레이션 완료!
   ```
5. Play 모드 종료 후 GameObject 삭제

#### 마이그레이션 기능
- ✅ 올바른 스키마로 테이블 재생성 (PRIMARY KEY AUTOINCREMENT)
- ✅ 기존 데이터 자동 백업 및 복원
- ✅ 한국 시간(KST) 기본값 적용
- ✅ 테이블이 없어도 안전하게 실행 가능

### 루틴 추가 UI 구성

루틴 추가 기능의 로직은 완성되었지만 Unity UI 구성이 필요합니다.

**가이드 문서**: `UNITY_UI_SETUP.md` 참고

**필요한 작업** (~20분):
1. + 버튼 생성 및 배치
2. AddRoutineDialog Panel 구성
3. InputField, Dropdown, Button 컴포넌트 연결
4. 테스트

### 빌드 주의사항
- **Windows**: StreamingAssets의 DB 파일이 읽기 전용
- **Android**: APK에 포함된 DB는 수정 불가하므로 앱 시작 시 내부 저장소로 복사 필요
- **시간대**: 모든 날짜/시간은 한국 시간(KST, UTC+9) 기준

## 코드 구조

### 핵심 클래스

#### DatabaseManager.cs
- 싱글톤 패턴으로 전역 DB 접근 제공
- UniTask 기반 비동기 쿼리
- 씬 전환 시에도 인스턴스 유지 (`DontDestroyOnLoad`)
- 타입 안전 데이터 읽기/쓰기

#### RoutineUIController.cs
- 앱 초기화 프로세스 관리
- DB와 UI 연결 역할
- 루틴 목록 로딩 및 새로고침
- 다이얼로그 관리

#### RoutineItemUI.cs
- 개별 루틴 아이템 UI 컴포넌트
- 체크박스 이벤트 처리
- DB 업데이트 실패 시 UI 롤백

#### AddRoutineDialog.cs
- 루틴 추가 다이얼로그 로직
- 입력 유효성 검사
- 타입/카테고리 변환 (한글 ↔ 영문)

#### DatabaseMigration.cs (일회성)
- DB 스키마 마이그레이션 스크립트
- 테이블 재생성 및 데이터 복원
- PRIMARY KEY AUTOINCREMENT 적용

#### SwipeUI.cs
- Scrollbar 기반 페이지 전환
- 스와이프 제스처 감지 및 처리
- 부드러운 애니메이션 (Lerp)

## 개발 히스토리

### 2025-10-29 (화)
- ✅ **루틴 추가 기능 완전 구현**
  - AddRoutineDialog UI 구성 및 Inspector 연결 완료
  - 루틴 추가 폼 (제목, 타입, 카테고리, 설명) 동작 테스트 완료
  - 입력 유효성 검사 및 데이터 저장 기능 정상 작동 확인

- ✅ **루틴 삭제 기능 완전 구현**
  - DatabaseManager에 DeleteRoutineAsync() 메서드 추가
  - 트랜잭션으로 루틴과 완료 기록 안전하게 동시 삭제
  - RoutineItemUI에 삭제 버튼 UI 요소 추가
  - 삭제 확인 다이얼로그 (ConfirmationDialog.cs) 컴포넌트 생성
  - 실수 방지를 위한 사용자 확인 절차 구현

- ✅ **코드 품질 개선**
  - AddRoutineDialog.cs에서 불필요한 using System; 지시문 제거
  - Unity 프로젝트 버전을 6000.2.8f1로 업데이트
  - CLAUDE.md 문서의 Unity 버전 정보 동기화

- ✅ **UI 아키텍처 확장**
  - 재사용 가능한 ConfirmationDialog 컴포넌트 설계
  - 콜백 기반 유연한 확인 다이얼로그 시스템
  - 삭제 후 자동 목록 새로고침 기능

### 2025-10-17 (목)
- ✅ **루틴 완료 체크박스 기능 구현**
  - 체크박스 UI와 DB 실시간 동기화
  - DB 업데이트 실패 시 UI 자동 롤백
  - RoutineItemUI.cs 컴포넌트 작성

- ✅ **DB 스키마 문제 해결**
  - CSV import의 PRIMARY KEY 누락 문제 발견
  - DatabaseMigration.cs 작성 (테이블 재생성 + 데이터 복원)
  - PRIMARY KEY AUTOINCREMENT 적용

- ✅ **타입 안전성 개선**
  - SQLite TEXT/INTEGER 자동 변환 처리
  - GetInt32() → GetValue() + Convert.ToInt32()

- ✅ **한국 시간(KST) 적용**
  - datetime('now', '+9 hours') 적용
  - 자정 기준 정확한 날짜 구분

- ✅ **루틴 추가 기능 기반 구현**
  - AddRoutineDialog.cs 로직 완성
  - RoutineUIController 연동
  - UNITY_UI_SETUP.md 작성 (UI 구성 가이드)

- ✅ **코드 정리**
  - 디버그 로그 제거
  - 레거시 메서드 삭제 (ToggleRoutineCompletionAsync)
  - 문서 업데이트 (README, CLAUDE.md)

### 2025-10-16 (수)
- ✅ Unity 버전 업그레이드 (6000.0.59f2 → 6000.2.8f1)
- ✅ .gitignore에 IDE 설정 파일 추가
- ✅ 프로젝트 README 작성
- ✅ GitHub 저장소 초기 설정
- ✅ Unity 프로젝트 생성
- ✅ SQLite DB 연결 (DatabaseManager.cs)
- ✅ 루틴 목록 조회 기능
- ✅ 스와이프 UI 네비게이션
- ✅ 로딩 화면

## 라이선스

개인 프로젝트 (비상업적 용도)

---

**마지막 업데이트**: 2025-10-29
