---
description: how to run the application on a new PC after cloning
---

To run the FeedbackTrack application on a new machine, follow these steps:

### 1. Backend Setup (.NET API)
1. **Navigate to the API folder**:
   ```powershell
   cd FeedbackTrack.API
   ```
2. **Configure Database**:
   Open `appsettings.json` and ensure the `DefaultConnection` matches the local SQL Server instance. 
   *(Example for SQL Express: `Server=.\SQLEXPRESS;Database=FeedbackTrackDB;Trusted_Connection=True;TrustServerCertificate=True;`)*

3. **Restore & Update Database**:
   Run these commands to install dependencies and create the database schema (including Views and Stored Procedures):
   ```powershell
   dotnet restore
   dotnet ef database update
   ```

4. **Run the API**:
   ```powershell
   dotnet run
   ```
   The API will start at `http://localhost:5002`.

---

### 2. Frontend Setup (Angular)
1. **Navigate to the Client folder**:
   ```powershell
   cd ../FeedbackTrack.Client
   ```
2. **Install Dependencies**:
   ```powershell
   npm install
   ```
3. **Run the Application**:
   ```powershell
   npm start
   ```
   The app will be available at `http://localhost:4200`.

---

### 3. Verify Login
Once running, you can log in with these seeded credentials:
- **Admin**: `admin@feedback.com` / `admin123`
- **Manager**: `manager@feedback.com` / `manager123`
- **Employee**: `employee@feedback.com` / `employee123`
