# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity-based Todo List and Routine Tracking application. Despite the project folder name "2048", this is a productivity app that allows users to:
- Create and manage daily routines
- Track completion of tasks over time (daily, weekly, monthly)
- Store data in an SQLite database

**Unity Version**: 6000.0.59f2 (Unity 6)

## Database Architecture

The application uses SQLite database stored at `Assets/StreamingAssets/ToDoList_DB.db`.

### Database Schema

**Routines Table:**
- `id` (INTEGER, PRIMARY KEY, AUTO INCREMENT) - Unique identifier
- `title` (TEXT, NOT NULL) - Name of the routine
- `type` (TEXT, NOT NULL) - Repetition type: "daily", "weekly", or "monthly"
- `category` (TEXT) - Optional categorization (e.g., "운동", "공부", "건강", "취미")
- `description` (TEXT) - Optional additional details
- `is_active` (BOOLEAN, DEFAULT TRUE) - Whether routine is currently active
- `created_at` (DATETIME) - Creation timestamp
- `updated_at` (DATETIME) - Last modification timestamp

**RoutineCompletions Table:**
- `id` (INTEGER, PRIMARY KEY, AUTO INCREMENT) - Unique identifier
- `routine_id` (INTEGER, NOT NULL, FOREIGN KEY) - References Routines.id
- `start_date` (DATE, NOT NULL) - Period start date (varies by type):
  - daily: the specific date (e.g., 2025-10-16)
  - weekly: Sunday of that week (e.g., 2025-10-12)
  - monthly: first day of month (e.g., 2025-10-01)
- `is_completed` (BOOLEAN, DEFAULT FALSE) - Completion status
- `completed_at` (DATETIME, NULLABLE) - Actual completion timestamp
- `updated_at` (DATETIME) - Last modification timestamp

### CSV Import Files

Sample data exists in two CSV files at the project root:
- `routines.csv` - Sample routine definitions
- `routine_completions.csv` - Sample completion records

To import into SQLite:
```bash
sqlite3 Assets/StreamingAssets/ToDoList_DB.db
.mode csv
.import routines.csv Routines
.import routine_completions.csv RoutineCompletions
```

## Project Structure

```
Assets/
├── ToDoListApp/           # Main application code
│   ├── DB_Test.cs         # Database connection testing
│   └── MainManager.cs     # Main game manager
├── Scenes/
│   ├── SampleScene.unity
│   └── TodoLIst.unity     # Main todo list scene
├── StreamingAssets/
│   └── ToDoList_DB.db     # SQLite database (runtime accessible)
└── Settings/              # Unity project settings
```

## Key Technologies

- **Unity Input System** (1.14.2) - For handling user interactions
- **Universal Render Pipeline** (17.0.4) - Unity's rendering pipeline
- **SQLite with Mono.Data.Sqlite** - For persistent data storage
- **Unity UI (UGUI)** (2.0.0) - For interface

## Database Connection Pattern

The database is accessed via:
```csharp
string dbname = "ToDoList_DB";
string connectionString = "URI=file:" + Application.streamingAssetsPath + "/" + dbname + ".db";
IDbConnection dbConnection = new SqliteConnection(connectionString);
dbConnection.Open();
```

**Important**: `Application.streamingAssetsPath` is platform-dependent. On Windows builds, files are read-only.

## Development Notes

### Opening the Project
1. Open Unity Hub
2. Add project from this directory
3. Unity version 6000.0.59f2 is required
4. The project will automatically load dependencies from Packages/manifest.json

### Testing Database Connection
- Attach `DB_Test.cs` to a GameObject in the scene
- Run the scene to test database connectivity
- Check Unity Console for connection errors

### Working with SQLite in Unity
- Database file must be in `Assets/StreamingAssets/` for runtime access
- Use `Mono.Data.Sqlite` namespace (included with Unity)
- Connection string uses `URI=file:` prefix
- Always properly close database connections to prevent locks

### Scene Organization
- `TodoLIst.unity` is the main application scene
- `SampleScene.unity` appears to be a test/template scene

## Language
The application is primarily in Korean (한국어), as evidenced by database categories and UI planning.
