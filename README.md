# FeedbackTrack
A role-based Feedback and Recognition platform built with .NET 8 (Web API) and Angular. Featuring a premium UI with role-specific dashboards for Employees, Managers, and Admins.

## 🚀 Foolproof Setup (New Machines)

To avoid "ghost files" and build conflicts on a new PC, follow these exact steps:

### 1. Initial Cleanup & Clone
1. Open a terminal in your project directory.
2. If a folder named `EmployeeFeedback` already exists, **delete it** manually first to ensure a fresh start.
3. Clone the repository:
   ```bash
   git clone https://github.com/prashantrajak1/EmployeeFeedback.git
   cd EmployeeFeedback
   ```

### 2. Backend Setup (.NET 8 & SQL Server)
1. **Navigate to API**: `cd FeedbackTrack.API`
2. **Database Config**: Open `appsettings.json` and ensure the `DefaultConnection` points to your local SQL Server instance (default is `USER\SQLEXPRESS`).
3. **Restore & Update**:
   ```bash
   dotnet restore
   dotnet ef database update
   ```
   *This command creates your tables, the user view (`vw_`), and the stored procedure (`sp_`).*
4. **Run**: `dotnet run` (Starts at `http://localhost:5002`)

### 3. Frontend Setup (Angular)
1. Open a **new** terminal in `FeedbackTrack.Client`.
2. **Install**: `npm install`
3. **Start**: `npm start` (Starts at `http://localhost:4200`)

---

## 🔐 Seeded Credentials
Use these accounts to explore the different dashboard roles:

| Role | Email | Password |
| :--- | :--- | :--- |
| **Admin** | `admin@feedback.com` | `admin123` |
| **Manager** | `manager@feedback.com` | `manager123` |
| **Employee** | `employee@feedback.com` | `employee123` |

---

## 🛠️ Troubleshooting
- **Build Errors (OpenApi Info)**: If you see errors about `OpenApiInfo` or `ParameterLocation`, ensure you have deleted any local `obj` or `bin` folders and run `dotnet restore`.
- **SQL Server Connection**: Ensure your SQL Server instance is running and the database name `FeedbackTrackDB` is correct in `appsettings.json`.
- **Port Conflicts**: Port 5002 (API) and 4200 (Client) must be available.
