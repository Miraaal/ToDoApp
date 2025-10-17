# 루틴 추가 기능 Unity UI 설정 가이드

## 📋 목차
1. [UI 구조 생성](#1-ui-구조-생성)
2. [다이얼로그 UI 만들기](#2-다이얼로그-ui-만들기)
3. [컴포넌트 연결](#3-컴포넌트-연결)
4. [테스트](#4-테스트)

---

## 1. UI 구조 생성

### 1-1. + 버튼 만들기

1. **Hierarchy**에서 Canvas 선택
2. 우클릭 → **UI → Button** 생성
3. 이름을 `AddRoutineButton`으로 변경
4. **Inspector**에서 설정:
   - **Rect Transform**:
     - Anchor: 우측 하단 (Right-Bottom)
     - Pos X: -50
     - Pos Y: 50
     - Width: 60
     - Height: 60
   - **Button (Script)** → Text 자식의 Text를 `+`로 변경

### 1-2. 다이얼로그 Panel 만들기

1. **Hierarchy**에서 Canvas 우클릭 → **UI → Panel** 생성
2. 이름을 `AddRoutineDialog`로 변경
3. **Inspector**에서:
   - **Rect Transform**: Stretch 전체 (Left, Top, Right, Bottom 모두 0)
   - **Image (Script)** → Color 알파값을 128로 (반투명 배경)

---

## 2. 다이얼로그 UI 만들기

### 2-1. 다이얼로그 컨테이너

`AddRoutineDialog` 하위에 Panel 하나 더 생성:

1. `AddRoutineDialog` 우클릭 → **UI → Panel**
2. 이름을 `DialogPanel`로 변경
3. **Inspector** 설정:
   - **Rect Transform**:
     - Anchor: Center-Middle
     - Pos X: 0
     - Pos Y: 0
     - Width: 400
     - Height: 350
   - **Image → Color**: 흰색 (255, 255, 255, 255)

### 2-2. 입력 필드들 만들기

`DialogPanel` 하위에 다음 UI 생성:

#### A. 제목 입력
1. `DialogPanel` 우클릭 → **UI → Input Field**
2. 이름: `TitleInput`
3. 위치: Y = 100, Width = 350, Height = 40
4. Placeholder Text: "루틴 이름을 입력하세요"

#### B. 타입 선택
1. `DialogPanel` 우클릭 → **UI → Dropdown**
2. 이름: `TypeDropdown`
3. 위치: Y = 40, Width = 350, Height = 40

#### C. 카테고리 선택
1. `DialogPanel` 우클릭 → **UI → Dropdown**
2. 이름: `CategoryDropdown`
3. 위치: Y = -20, Width = 350, Height = 40

#### D. 설명 입력
1. `DialogPanel` 우클릭 → **UI → Input Field**
2. 이름: `DescriptionInput`
3. 위치: Y = -80, Width = 350, Height = 40
4. Placeholder Text: "설명 (선택사항)"

### 2-3. 버튼 만들기

`DialogPanel` 하위에:

#### 확인 버튼
1. 우클릭 → **UI → Button**
2. 이름: `ConfirmButton`
3. 위치: X = -100, Y = -140, Width = 100, Height = 40
4. Text: "추가"

#### 취소 버튼
1. 우클릭 → **UI → Button**
2. 이름: `CancelButton`
3. 위치: X = 100, Y = -140, Width = 100, Height = 40
4. Text: "취소"

---

## 3. 컴포넌트 연결

### 3-1. AddRoutineDialog 스크립트 추가

1. **Hierarchy**에서 `AddRoutineDialog` (최상위) 선택
2. **Inspector** → **Add Component** → `AddRoutineDialog` 스크립트 선택
3. **Inspector**에서 각 필드에 드래그 앤 드롭:
   - **Title Input** → `TitleInput` 오브젝트
   - **Type Dropdown** → `TypeDropdown` 오브젝트
   - **Category Dropdown** → `CategoryDropdown` 오브젝트
   - **Description Input** → `DescriptionInput` 오브젝트
   - **Confirm Button** → `ConfirmButton` 오브젝트
   - **Cancel Button** → `CancelButton` 오브젝트

### 3-2. RoutineUIController 연결

1. **Hierarchy**에서 `RoutineUIController`가 있는 GameObject 선택
2. **Inspector**에서:
   - **Add Routine Button** → `AddRoutineButton` 드래그
   - **Add Routine Dialog** → `AddRoutineDialog` (최상위) 드래그

---

## 4. 테스트

### 4-1. 실행 전 확인사항

**Hierarchy 구조:**
```
Canvas
├─ [기존 UI들...]
├─ AddRoutineButton
└─ AddRoutineDialog
    ├─ Image (반투명 배경)
    └─ DialogPanel
        ├─ TitleInput
        ├─ TypeDropdown
        ├─ CategoryDropdown
        ├─ DescriptionInput
        ├─ ConfirmButton
        └─ CancelButton
```

**Inspector 연결 확인:**
- ✅ AddRoutineDialog 스크립트의 모든 필드 연결됨
- ✅ RoutineUIController의 Button, Dialog 연결됨

### 4-2. 테스트 시나리오

1. **Play 모드 실행**
2. **+ 버튼 클릭** → 다이얼로그가 나타남
3. **정보 입력**:
   - 제목: "아침 운동"
   - 타입: "매일"
   - 카테고리: "운동"
4. **추가 버튼 클릭**
5. **Console 확인**: "✅ 루틴 추가 성공!" 메시지
6. **루틴 목록** 새로고침되어 새 항목 표시

### 4-3. 문제 해결

**다이얼로그가 안 열려요:**
- RoutineUIController의 `addRoutineDialog` 필드 연결 확인
- Console에 에러 메시지 확인

**버튼이 작동 안 해요:**
- AddRoutineDialog 스크립트의 Button 필드 연결 확인
- Initialize() 메서드가 호출되는지 확인

**루틴이 추가 안 돼요:**
- DatabaseManager가 초기화되었는지 확인
- DB 마이그레이션이 완료되었는지 확인

---

## 📸 참고 스크린샷 위치

각 단계별로 Unity 에디터 스크린샷이 있으면 좋습니다:
- UI 구조 완성 후
- Inspector 설정 완성 후
- 실행 화면

---

## 🎯 다음 단계

기본 기능이 작동하면:
1. ✅ **레이아웃 개선** - 더 예쁘게
2. ✅ **유효성 검사 강화** - 빈 값 체크, 중복 체크
3. ✅ **애니메이션 추가** - 다이얼로그 열기/닫기
4. ✅ **키보드 단축키** - Enter로 추가, ESC로 취소

---

**작성일**: 2025-10-17
