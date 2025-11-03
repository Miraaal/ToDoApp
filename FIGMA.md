# Figma 프로토타입 가이드

> Unity 기반 Todo/가계부 앱 UI/UX 프로토타입 제작 가이드

**작성일**: 2025-11-03
**대상**: Figma를 사용한 모바일 앱 프로토타입 디자인

---

## 목차
1. [화면 구조](#1-화면-구조)
2. [디자인 시스템](#2-디자인-시스템)
3. [컴포넌트 설계](#3-컴포넌트-설계)
4. [화면별 상세 레이아웃](#4-화면별-상세-레이아웃)
5. [프로토타입 인터랙션](#5-프로토타입-인터랙션)
6. [샘플 데이터](#6-샘플-데이터)

---

## 1. 화면 구조

### 1.1 Figma Frame 크기

**권장 모바일 화면 크기:**
- **iPhone 13 Pro**: 390 x 844 (권장)
- **iPhone SE**: 375 x 667
- **Android (일반)**: 360 x 800

**선택 가이드:**
- 1개 크기로 시작: iPhone 13 Pro (390 x 844)
- 반응형 테스트 필요시: 여러 크기 동시 작업

### 1.2 필수 화면 목록

**메인 화면 (5개)**
1. `Screen_Routine` - 루틴 화면
2. `Screen_Diary` - 일기 화면
3. `Screen_AccountBook` - 가계부 화면 (신규)
4. `Screen_Shop` - 상점 화면
5. `Screen_Statistics` - 통계 화면

**가계부 서브 화면 (2개)**
6. `Screen_AccountBook_Stats` - 가계부 통계 화면
7. `Dialog_Transaction` - 거래 추가/수정 다이얼로그

**공통 다이얼로그**
8. `Dialog_Confirmation` - 확인 다이얼로그
9. `Dialog_AddRoutine` - 루틴 추가 다이얼로그
10. `Dialog_EditRoutine` - 루틴 수정 다이얼로그

---

## 2. 디자인 시스템

### 2.1 컬러 팔레트

#### Primary Colors
```
- Primary: #6200EE (보라색 - 메인 액션)
- Primary Variant: #3700B3 (어두운 보라색)
- Secondary: #03DAC6 (청록색 - 보조 액션)
- Secondary Variant: #018786
```

#### 가계부 전용 색상
```
- Income (수입): #2196F3 (파란색) 또는 #4CAF50 (녹색)
- Expense (지출): #F44336 (빨간색) 또는 #FF5252 (밝은 빨간색)
- Positive Balance (양수): #4CAF50 (녹색)
- Negative Balance (음수): #F44336 (빨간색)
- Budget Warning (예산 주의): #FF9800 (주황색)
- Budget Over (예산 초과): #F44336 (빨간색)
```

#### Background & Surface
```
- Background: #FFFFFF (흰색)
- Surface: #F5F5F5 (연한 회색)
- Card Background: #FFFFFF
- Divider: #E0E0E0
```

#### Text Colors
```
- Primary Text: #212121 (검정에 가까운 회색)
- Secondary Text: #757575 (회색)
- Disabled Text: #BDBDBD (연한 회색)
- White Text: #FFFFFF
```

#### Semantic Colors
```
- Success: #4CAF50
- Error: #F44336
- Warning: #FF9800
- Info: #2196F3
```

### 2.2 타이포그래피

**폰트 패밀리:**
- 한글: Pretendard, Noto Sans KR, Apple SD Gothic Neo
- 영문/숫자: SF Pro, Roboto

**텍스트 스타일:**
```
- H1 (페이지 제목): 24px / Bold / #212121
- H2 (섹션 제목): 20px / Bold / #212121
- H3 (카드 제목): 18px / SemiBold / #212121
- Body1 (본문): 16px / Regular / #212121
- Body2 (보조 본문): 14px / Regular / #757575
- Caption (캡션): 12px / Regular / #757575
- Button (버튼): 16px / SemiBold / #FFFFFF
- Amount (금액): 20px / Bold / Income/Expense 색상
- Amount Large (큰 금액): 28px / Bold / Income/Expense 색상
```

### 2.3 간격 시스템 (Spacing)

**8pt Grid 시스템 사용:**
```
- XXS: 4px
- XS: 8px
- S: 12px
- M: 16px
- L: 24px
- XL: 32px
- XXL: 48px
```

**적용 예시:**
- 카드 내부 패딩: 16px (M)
- 카드 간 간격: 12px (S)
- 화면 좌우 마진: 16px (M)
- 섹션 간 간격: 24px (L)

### 2.4 그림자 (Shadows)

**Elevation 레벨:**
```
- Level 1 (카드): 0px 2px 4px rgba(0,0,0,0.1)
- Level 2 (부유 버튼): 0px 4px 8px rgba(0,0,0,0.15)
- Level 3 (다이얼로그): 0px 8px 16px rgba(0,0,0,0.2)
```

### 2.5 모서리 반경 (Border Radius)

```
- Small (버튼, 태그): 8px
- Medium (카드): 12px
- Large (다이얼로그): 16px
- Circle (아이콘 배경): 50%
```

---

## 3. 컴포넌트 설계

### 3.1 가계부 전용 컴포넌트

#### 1. Summary Card (요약 카드)
**크기**: 343 x 80 (전체 너비 - 32px 마진)

**구조:**
```
┌─────────────────────────────┐
│ [아이콘] 총 수입            ↗ │
│                               │
│ ₩1,234,567                    │
│ 전월 대비 +5.2%               │
└─────────────────────────────┘
```

**변형 (Variants):**
- Type: income / expense / balance
- State: default / positive / negative

**Auto Layout 설정:**
- Direction: Vertical
- Spacing: 8px
- Padding: 16px
- Fill: Container

#### 2. Transaction Item (거래 아이템)
**크기**: 343 x 72

**구조:**
```
┌─────────────────────────────┐
│ [📌] 식비      -12,000원     │
│     점심 식사                 │
│     10월 15일                 │
└─────────────────────────────┘
```

**컴포넌트 구성:**
- Category Icon (32x32)
- Category Name (Body1)
- Amount (Amount, 색상 조건부)
- Description (Body2)
- Date (Caption)

**Variants:**
- Type: income / expense

#### 3. Category Icon (카테고리 아이콘)
**크기**: 40 x 40

**아이콘 목록 (지출):**
- 🍽️ 식비
- 🚗 교통
- 🛍️ 쇼핑
- 🎬 문화
- 🏠 주거
- 💊 의료
- 📚 교육
- ❓ 기타

**아이콘 목록 (수입):**
- 💰 급여
- 💵 부수입
- ❓ 기타

**스타일:**
- Background: 카테고리별 색상 (연한 톤)
- Icon Size: 24x24
- Border Radius: 12px

#### 4. Budget Progress Bar (예산 진행바)
**크기**: 343 x 60

**구조:**
```
┌─────────────────────────────┐
│ 식비                          │
│ ████████░░░░░░░░ 75%          │
│ 750,000원 / 1,000,000원      │
└─────────────────────────────┘
```

**Progress 색상:**
- 0-70%: #4CAF50 (녹색)
- 71-90%: #FF9800 (주황색)
- 91-100%: #F44336 (빨간색)
- 100% 초과: #D32F2F (어두운 빨간색)

#### 5. Month Selector (월 선택기)
**크기**: 343 x 44

**구조:**
```
┌─────────────────────────────┐
│  ◀  2025년 11월  ▶           │
└─────────────────────────────┘
```

### 3.2 공통 컴포넌트

#### 1. Bottom Tab Bar (하단 탭 바)
**크기**: 390 x 72

**탭 목록 (5개):**
1. 루틴 (📋 or icon)
2. 일기 (📝 or icon)
3. 가계부 (💰 or icon) - 신규
4. 상점 (🏪 or icon)
5. 통계 (📊 or icon)

**State:**
- Active: Primary Color, Bold
- Inactive: #757575, Regular

#### 2. Primary Button
**크기**: 343 x 48

**Variants:**
- Type: primary / secondary / outlined
- State: default / hover / pressed / disabled

#### 3. Input Field
**크기**: 343 x 56

**Variants:**
- Type: text / number / date
- State: default / focused / error / disabled

#### 4. Dialog Container
**크기**: 343 x Auto

**구조:**
- Header: 제목 (H2)
- Content: 다이얼로그 내용
- Actions: 버튼 영역 (오른쪽 정렬)

**Padding:**
- All sides: 24px
- Button spacing: 8px

---

## 4. 화면별 상세 레이아웃

### 4.1 가계부 메인 화면 (Screen_AccountBook)

**화면 크기**: 390 x 844

#### 상단 영역 (Header)
**높이**: 56px
```
┌──────────────────────────────────┐
│  ◀  2025년 11월  ▶    [필터 아이콘]│
└──────────────────────────────────┘
```

**요소:**
- Month Selector (중앙)
- Filter Button (우측 상단, 24x24)

#### 요약 카드 영역
**높이**: Auto (3개 카드 + 간격)
```
┌──────────────────────────────────┐
│                                  │
│  ┌────────────────────────────┐  │
│  │ 💵 총 수입                  │  │
│  │ ₩3,500,000      ↗ +10%    │  │
│  └────────────────────────────┘  │
│                                  │
│  ┌────────────────────────────┐  │
│  │ 💸 총 지출                  │  │
│  │ ₩2,800,000      ↘ +5%     │  │
│  └────────────────────────────┘  │
│                                  │
│  ┌────────────────────────────┐  │
│  │ 💰 수지 차액                │  │
│  │ ₩700,000        ↗         │  │
│  └────────────────────────────┘  │
│                                  │
└──────────────────────────────────┘
```

**간격:**
- 좌우 마진: 16px
- 카드 간 간격: 12px
- 상단 마진: 16px

#### 거래 내역 영역
**높이**: 나머지 영역 (스크롤 가능)
```
┌──────────────────────────────────┐
│  거래 내역                 [통계→]  │
│                                  │
│  ━━━ 11월 15일 (금) ━━━          │
│                                  │
│  🍽️ 식비         -12,000원       │
│     점심 식사                     │
│                                  │
│  🚗 교통          -2,500원        │
│     지하철                        │
│                                  │
│  ━━━ 11월 14일 (목) ━━━          │
│                                  │
│  💰 급여        +3,500,000원     │
│     11월 급여                     │
│                                  │
│  🛍️ 쇼핑         -45,000원       │
│     옷 구매                       │
│                                  │
└──────────────────────────────────┘
```

**요소:**
- 섹션 헤더: "거래 내역" (H3) + 통계 버튼
- 날짜 구분선 (Caption, 중앙 정렬)
- Transaction Item 목록

#### 하단 액션 버튼
**높이**: 56px (Bottom Tab Bar 위)
```
┌──────────────────────────────────┐
│                    [➕]           │
└──────────────────────────────────┘
```

**요소:**
- Floating Action Button (56x56)
- Position: 우측 하단, 16px margin

#### 하단 탭 바
**높이**: 72px
```
┌──────────────────────────────────┐
│  📋   📝   💰   🏪   📊          │
│ 루틴  일기  가계부  상점  통계    │
└──────────────────────────────────┘
```

---

### 4.2 거래 추가/수정 다이얼로그 (Dialog_Transaction)

**크기**: 343 x Auto (중앙 정렬)

#### 레이아웃
```
┌─────────────────────────────────┐
│  거래 추가                    [X] │
│                                 │
│  ┌─────────┬─────────┐          │
│  │  수입   │  지출   │ (탭)     │
│  └─────────┴─────────┘          │
│                                 │
│  금액                            │
│  ┌─────────────────────────┐    │
│  │ ₩ 0                     │    │
│  └─────────────────────────┘    │
│                                 │
│  카테고리                        │
│  ┌─────────────────────────┐    │
│  │ 식비              ▼     │    │
│  └─────────────────────────┘    │
│                                 │
│  날짜                            │
│  ┌─────────────────────────┐    │
│  │ 2025-11-03       📅     │    │
│  └─────────────────────────┘    │
│                                 │
│  메모 (선택)                     │
│  ┌─────────────────────────┐    │
│  │                         │    │
│  └─────────────────────────┘    │
│                                 │
│                   [취소] [저장]  │
└─────────────────────────────────┘
```

**요소:**
- Dialog Header: 제목 + 닫기 버튼
- Tab Switcher: 수입/지출
- Amount Input: 숫자 입력 (큰 폰트)
- Category Dropdown
- Date Picker
- Memo Input (선택, 여러 줄)
- Action Buttons: 취소 (Outlined), 저장 (Primary)

**Padding:**
- Container: 24px
- Input spacing: 16px

---

### 4.3 가계부 통계 화면 (Screen_AccountBook_Stats)

**화면 크기**: 390 x 844

#### 상단 영역 (Header)
```
┌──────────────────────────────────┐
│  ◀  통계                          │
└──────────────────────────────────┘
```

#### 기간 선택
```
┌──────────────────────────────────┐
│  ┌──────┬──────┬──────┬──────┐   │
│  │이번 달│지난 달│3개월│  연간│   │
│  └──────┴──────┴──────┴──────┘   │
└──────────────────────────────────┘
```

#### 수입/지출 추이 차트
```
┌──────────────────────────────────┐
│  월별 수입/지출                    │
│                                  │
│  [라인 차트 영역]                 │
│  높이: 200px                      │
│                                  │
└──────────────────────────────────┘
```

**차트 설명:**
- 파란색 선: 수입
- 빨간색 선: 지출
- X축: 월 (1월 ~ 12월)
- Y축: 금액 (단위: 만원)

#### 카테고리별 지출 분석
```
┌──────────────────────────────────┐
│  카테고리별 지출                  │
│                                  │
│  [파이 차트]                      │
│  크기: 200x200                    │
│                                  │
│  🍽️ 식비      450,000원 (35%)   │
│  ████████████░░░░░░░             │
│                                  │
│  🚗 교통      300,000원 (23%)   │
│  ████████░░░░░░░░░░              │
│                                  │
│  🛍️ 쇼핑      200,000원 (15%)   │
│  ██████░░░░░░░░░░░░              │
│                                  │
└──────────────────────────────────┘
```

#### 예산 관리
```
┌──────────────────────────────────┐
│  예산 현황              [예산 설정] │
│                                  │
│  식비                             │
│  ████████████████░░ 80%          │
│  800,000원 / 1,000,000원         │
│                                  │
│  교통                             │
│  ██████████████████░ 95%         │
│  285,000원 / 300,000원  ⚠️       │
│                                  │
│  쇼핑                             │
│  ██████████████████████ 105% 🚨  │
│  525,000원 / 500,000원           │
│                                  │
└──────────────────────────────────┘
```

---

## 5. 프로토타입 인터랙션

### 5.1 화면 전환 (Screen Transitions)

#### 하단 탭 전환
```
트리거: 탭 아이콘 클릭
액션: Navigate to → 해당 화면
애니메이션: Instant (또는 Slide 100ms)
```

#### 다이얼로그 표시
```
트리거: + 버튼 클릭
액션: Open Overlay → Dialog_Transaction
애니메이션: Dissolve 200ms
배경: #000000, Opacity 50%
```

#### 다이얼로그 닫기
```
트리거:
  1. X 버튼 클릭
  2. 취소 버튼 클릭
  3. 배경 클릭
액션: Close Overlay
애니메이션: Dissolve 200ms
```

### 5.2 상태 변경 (State Changes)

#### 탭 활성화/비활성화
```
트리거: 탭 클릭
액션: Change to → Active Variant
```

#### 버튼 Hover/Press
```
트리거: Mouse Enter / Click
액션: Change to → Hover/Pressed Variant
```

#### 입력 필드 포커스
```
트리거: Click
액션: Change to → Focused Variant
효과: Border 색상 변경 (Primary)
```

### 5.3 스크롤 영역

**스크롤 가능 영역:**
- 거래 내역 목록
- 통계 화면 전체

**설정:**
- Overflow Behavior: Vertical Scrolling
- Clip Content: Yes

### 5.4 월 이동 인터랙션

```
트리거: ◀ 또는 ▶ 버튼 클릭
액션:
  1. 월 텍스트 변경 (예: 11월 → 10월)
  2. 거래 내역 데이터 변경
애니메이션: Smart Animate 300ms
```

### 5.5 필터 인터랙션

```
트리거: 필터 버튼 클릭
액션: Open Overlay → Filter Dialog
옵션:
  - 전체
  - 수입만
  - 지출만
```

---

## 6. 샘플 데이터

### 6.1 거래 내역 샘플 (11월)

#### 11월 15일 (금)
```
1. 식비 | -12,000원 | 점심 식사 | 🍽️
2. 교통 | -2,500원 | 지하철 | 🚗
3. 식비 | -5,500원 | 카페 | 🍽️
```

#### 11월 14일 (목)
```
1. 급여 | +3,500,000원 | 11월 급여 | 💰
2. 쇼핑 | -45,000원 | 옷 구매 | 🛍️
3. 식비 | -18,000원 | 저녁 회식 | 🍽️
```

#### 11월 13일 (수)
```
1. 주거 | -650,000원 | 월세 | 🏠
2. 주거 | -120,000원 | 관리비 | 🏠
3. 교통 | -2,500원 | 지하철 | 🚗
4. 식비 | -8,000원 | 점심 | 🍽️
```

#### 11월 12일 (화)
```
1. 교육 | -35,000원 | 온라인 강의 | 📚
2. 식비 | -6,500원 | 카페 | 🍽️
3. 교통 | -15,000원 | 택시 | 🚗
```

#### 11월 11일 (월)
```
1. 문화 | -25,000원 | 영화 관람 | 🎬
2. 식비 | -22,000원 | 외식 | 🍽️
3. 쇼핑 | -15,000원 | 생활용품 | 🛍️
```

#### 11월 10일 (일)
```
1. 부수입 | +150,000원 | 프리랜서 수입 | 💵
2. 문화 | -80,000원 | 콘서트 티켓 | 🎬
3. 식비 | -30,000원 | 브런치 | 🍽️
```

### 6.2 월별 통계 샘플

#### 11월 (현재)
```
- 총 수입: 3,650,000원
  - 급여: 3,500,000원
  - 부수입: 150,000원

- 총 지출: 2,845,000원
  - 식비: 850,000원 (29.8%)
  - 주거: 770,000원 (27.0%)
  - 쇼핑: 525,000원 (18.4%)
  - 교통: 285,000원 (10.0%)
  - 문화: 185,000원 (6.5%)
  - 교육: 150,000원 (5.3%)
  - 의료: 80,000원 (2.8%)

- 수지 차액: +805,000원
```

#### 10월
```
- 총 수입: 3,500,000원
- 총 지출: 2,650,000원
- 수지 차액: +850,000원
```

#### 9월
```
- 총 수입: 3,500,000원
- 총 지출: 2,800,000원
- 수지 차액: +700,000원
```

### 6.3 예산 샘플 (11월)

```
카테고리  | 예산          | 사용          | 잔여          | 사용률
---------|--------------|--------------|--------------|-------
식비      | 1,000,000원  | 850,000원    | 150,000원    | 85%
교통      | 300,000원    | 285,000원    | 15,000원     | 95%
쇼핑      | 500,000원    | 525,000원    | -25,000원    | 105% 🚨
문화      | 200,000원    | 185,000원    | 15,000원     | 93%
교육      | 200,000원    | 150,000원    | 50,000원     | 75%
의료      | 100,000원    | 80,000원     | 20,000원     | 80%
```

---

## 7. Figma 작업 팁

### 7.1 프레임 구조 (권장)

```
📁 ToDoApp Prototype
  📁 Design System
    📁 Colors (Color Styles)
    📁 Typography (Text Styles)
    📁 Components
      📁 AccountBook
        - Summary Card
        - Transaction Item
        - Category Icon
        - Budget Progress Bar
        - Month Selector
      📁 Common
        - Bottom Tab Bar
        - Buttons
        - Input Fields
        - Dialogs
  📁 Screens
    - Screen_Routine
    - Screen_Diary
    - Screen_AccountBook ⭐
    - Screen_AccountBook_Stats ⭐
    - Screen_Shop
    - Screen_Statistics
  📁 Dialogs
    - Dialog_Transaction ⭐
    - Dialog_Confirmation
    - Dialog_AddRoutine
  📁 Prototypes
    - Flow_AccountBook (연결된 화면들)
```

### 7.2 Auto Layout 활용

**장점:**
- 컴포넌트 크기 자동 조정
- 반응형 디자인 구현
- 유지보수 용이

**적용 대상:**
- 모든 카드 컴포넌트
- 버튼
- 입력 필드
- 목록 아이템
- 다이얼로그

### 7.3 Component Variants

**활용 예:**
- Button: primary/secondary/outlined x default/hover/pressed/disabled
- Transaction Item: income/expense
- Summary Card: income/expense/balance x positive/negative
- Tab: active/inactive

### 7.4 Styles 설정

**Color Styles:**
- Primary, Secondary 색상
- Background, Surface 색상
- Income, Expense, Balance 색상
- Text 색상 (Primary, Secondary, Disabled)

**Text Styles:**
- H1, H2, H3
- Body1, Body2
- Caption
- Button
- Amount (Small, Large)

**Effect Styles:**
- Shadow Elevation 1, 2, 3

### 7.5 플러그인 추천

**아이콘:**
- Iconify (무료 아이콘)
- Material Design Icons

**차트:**
- Chart (차트 생성)
- Chartify

**데이터:**
- Content Reel (샘플 데이터 생성)
- Google Sheets Sync (실제 데이터 연동)

**프로토타입:**
- Overflow (플로우 다이어그램)
- Stark (접근성 검사)

---

## 8. 체크리스트

### 디자인 시작 전
- [ ] Figma 파일 생성
- [ ] Frame 크기 설정 (390 x 844)
- [ ] Color Styles 정의
- [ ] Text Styles 정의
- [ ] 8pt Grid 설정

### 컴포넌트 제작
- [ ] Summary Card 컴포넌트
- [ ] Transaction Item 컴포넌트
- [ ] Category Icon 세트
- [ ] Budget Progress Bar 컴포넌트
- [ ] Month Selector 컴포넌트
- [ ] Bottom Tab Bar (5개 탭)
- [ ] Button 컴포넌트 (Variants)
- [ ] Input Field 컴포넌트

### 화면 제작
- [ ] Screen_AccountBook (가계부 메인)
- [ ] Dialog_Transaction (거래 추가)
- [ ] Screen_AccountBook_Stats (통계)

### 프로토타입 연결
- [ ] 하단 탭 전환 설정
- [ ] 다이얼로그 열기/닫기
- [ ] 월 이동 버튼
- [ ] 필터 버튼
- [ ] 통계 화면 이동

### 샘플 데이터 입력
- [ ] 거래 내역 10개 이상
- [ ] 요약 카드 금액
- [ ] 차트 데이터
- [ ] 예산 데이터

### 최종 검토
- [ ] 간격/정렬 일관성
- [ ] 색상 일관성
- [ ] 폰트 크기/무게 일관성
- [ ] 인터랙션 동작 확인
- [ ] 모바일 크기 적합성

---

**문서 버전**: 1.0
**최종 수정**: 2025-11-03
