---
description: how to sync the latest changes from GitHub to an existing local setup
---

To update your local application with the latest changes (including new stored procedures), follow these steps on your other PC:

### 1. Get the latest code
Open a terminal in the root of the `FeedbackTrack` project and run:
```powershell
git pull origin main
```

### 2. Update the Database
The new stored procedures were added via an EF Core migration. You must apply this migration to your local SQL Server:
1. **Navigate to the API folder**:
   ```powershell
   cd FeedbackTrack.API
   ```
2. **Apply Migrations**:
   ```powershell
   dotnet ef database update
   ```

### 3. (Optional) Run Seed Script
If you need to ensure the default users (Admin, Manager, Employee) and initial departments/roles are present:
```powershell
sqlcmd -S .\SQLEXPRESS -E -i "../seed_users.sql"
```
*(Note: Replace `.\SQLEXPRESS` with your actual server name if different.)*

### 4. Restart the Application
- **API**: `dotnet run` in `FeedbackTrack.API`
- **Frontend**: `npm start` in `FeedbackTrack.Client`
