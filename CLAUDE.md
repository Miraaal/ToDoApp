# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity-based Todo List and Routine Tracking application. Despite the project folder name "2048", this is a productivity app that allows users to:
- Create and manage daily routines
- Track completion of tasks over time (daily, weekly, monthly)
- Store data in an SQLite database

**Unity Version**: 6000.2.8f1 (Unity 6)

## Development Commands

### Unity Project Setup
```bash
# Open Unity Hub and add this directory as a project
# Unity 6000.2.8f1 is required
# Main scene: Assets/Scenes/TodoLIst.unity
```

### Database Operations
```bash
# Create/migrate database schema (run in Unity Editor)
# 1. Create empty GameObject in scene
# 2. Attach DatabaseMigration.cs component  
# 3. Enter Play mode to execute migration
# 4. Check console for "🎉 데이터베이스 마이그레이션 완료!"

# Manual database access (if needed)
sqlite3 Assets/StreamingAssets/ToDoList_DB.db
```

### Testing
```bash
# Database connection test
# 1. Attach DB_Test.cs to GameObject in scene
# 2. Run scene and check Unity Console for connection status
```

## Database Architecture

The application uses SQLite database stored at `Assets/StreamingAssets/ToDoList_DB.db`.

### Database Schema

**IMPORTANT: Table names are lowercase with underscores:**
- Actual table names in database: `routines` and `routine_completions`

**routines Table:**
- `id` (INTEGER, PRIMARY KEY, AUTO INCREMENT) - Unique identifier
- `title` (TEXT, NOT NULL) - Name of the routine
- `type` (TEXT, NOT NULL) - Repetition type: "daily", "weekly", or "monthly"
- `category` (TEXT) - Optional categorization (e.g., "운동", "공부", "건강", "취미")
- `description` (TEXT) - Optional additional details
- `is_active` (BOOLEAN, DEFAULT TRUE) - Whether routine is currently active
- `created_at` (TEXT) - Creation timestamp (KST, UTC+9)
- `updated_at` (TEXT) - Last modification timestamp (KST, UTC+9)

**routine_completions Table:**
- `id` (INTEGER, PRIMARY KEY, AUTO INCREMENT) - Unique identifier
- `routine_id` (INTEGER, NOT NULL, FOREIGN KEY) - References routines.id
- `start_date` (TEXT, NOT NULL) - Period start date (varies by type):
  - daily: the specific date (e.g., 2025-10-16)
  - weekly: Sunday of that week (e.g., 2025-10-12)
  - monthly: first day of month (e.g., 2025-10-01)
- `is_completed` (INTEGER, DEFAULT 0) - Completion status (0/1)
- `completed_at` (TEXT, NULLABLE) - Actual completion timestamp (KST, UTC+9)
- `updated_at` (TEXT) - Last modification timestamp (KST, UTC+9)

### Database Setup

**IMPORTANT:** Do not use direct CSV import as it creates tables without PRIMARY KEY or AUTOINCREMENT!

#### Recommended Setup (Proper Schema):
```bash
# Run the SQL script to create tables with correct schema
sqlite3 Assets/StreamingAssets/ToDoList_DB.db < recreate_tables.sql
```

The `recreate_tables.sql` file:
- Creates tables with proper PRIMARY KEY AUTOINCREMENT
- Backs up existing data before recreation
- Restores data after creating proper schema

#### CSV Import Files (Reference Only):
Sample data exists in two CSV files at the project root:
- `routines.csv` - Sample routine definitions
- `routine_completions.csv` - Sample completion records

**WARNING:** Direct CSV import (`.import` command) creates tables without constraints and should be avoided.

## Code Architecture

### Core Components

**DatabaseManager.cs** (Singleton)
- Manages SQLite connection lifecycle using `Mono.Data.Sqlite`
- All database operations are async using `UniTask`
- Survives scene transitions with `DontDestroyOnLoad`
- Key methods: `AddRoutineAsync()`, `DeleteRoutineAsync()`, `GetActiveRoutinesAsync()`

**RoutineUIController.cs** (Main Controller)
- Orchestrates UI initialization and data loading
- Connects UI events to database operations
- Manages loading states and error handling
- Controls dialog lifecycles (AddRoutineDialog, ConfirmationDialog)

**UI Component Hierarchy:**
```
RoutineUIController (Scene Controller)
├── AddRoutineDialog (Modal for adding routines)
├── ConfirmationDialog (Reusable confirmation popup)  
└── RoutineItemUI (Prefab instances for each routine)
    ├── Completion Toggle (checkbox with DB sync)
    └── Delete Button (triggers confirmation flow)
```

### Data Flow Pattern

1. **Initialization**: `RoutineUIController.Start()` → `DatabaseManager.ConnectDatabaseAsync()`
2. **Data Loading**: `LoadRoutinesAsync()` → `GetActiveRoutinesAsync()` + `GetAllRoutineCompletionStatusAsync()`
3. **UI Updates**: Dynamic prefab instantiation with real-time completion status
4. **User Actions**: UI events → Controller methods → Database operations → UI refresh

### Key Technologies

- **UniTask** (Cysharp) - Async/await for Unity without coroutines
- **SQLite with Mono.Data.Sqlite** - Local database with SQL parameterization
- **Unity Input System** (1.14.2) - Touch/mouse input handling
- **Universal Render Pipeline** (17.0.4) - Unity's modern rendering

## Database Connection Pattern

The database is accessed via:
```csharp
string dbname = "ToDoList_DB";
string connectionString = "URI=file:" + Application.streamingAssetsPath + "/" + dbname + ".db";
IDbConnection dbConnection = new SqliteConnection(connectionString);
dbConnection.Open();
```

**Important**: `Application.streamingAssetsPath` is platform-dependent. On Windows builds, files are read-only.

## Critical Development Patterns

### Database Transaction Safety
All database mutations use transactions for data integrity:
```csharp
var transaction = dbConnection.BeginTransaction();
try {
    // Multiple related operations
    transaction.Commit();
} catch {
    transaction.Rollback();
    throw;
}
```

### UI-Database Synchronization  
UI updates follow optimistic updates with rollback on failure:
```csharp
// Update UI immediately
toggle.SetIsOnWithoutNotify(newValue);

// Attempt database update
bool success = await DatabaseManager.Instance.SetRoutineCompletionAsync(...);

// Rollback UI if DB update failed
if (!success) {
    toggle.SetIsOnWithoutNotify(!newValue);
}
```

### Routine Type Date Calculations
Date handling varies by routine type:
- **Daily**: Current date (`yyyy-MM-dd`)
- **Weekly**: Sunday of current week  
- **Monthly**: First day of current month

All timestamps use Korean timezone (UTC+9): `datetime('now', '+9 hours')`

### Dialog Management Pattern
All dialogs follow this lifecycle:
1. Initialize with controller reference
2. `gameObject.SetActive(false)` by default
3. `Show()` method activates and configures
4. User action triggers callback
5. `Hide()` method deactivates and clears callbacks

## Important Constraints

- **Database Location**: Must be in `Assets/StreamingAssets/` for runtime access
- **Platform Differences**: Windows builds have read-only StreamingAssets
- **Schema Requirements**: Tables must have `PRIMARY KEY AUTOINCREMENT` 
- **Language**: UI and database content primarily in Korean (한국어)
- **Time Zone**: All operations assume Korean Standard Time (KST, UTC+9)
