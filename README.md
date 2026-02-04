# FeedbackTrack

A role-based Feedback and Recognition platform built with .NET 8 (Web API) and Angular.

## Prerequisites

Before running the application, ensure you have the following installed on your device:

- **Git**
- **.NET 8 SDK**
- **Node.js** (v18 or higher recommended)
- **Angular CLI** (`npm install -g @angular/cli`)

## Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/prashantrajak1/EmployeeFeedback.git
cd EmployeeFeedback
```

### 2. Backend Setup (.NET API)
Open a terminal in the `FeedbackTrack.API` directory.

```bash
cd FeedbackTrack.API
# Restore dependencies
dotnet restore
# Initialize the database (SQLite)
dotnet ef database update
# Run the API
dotnet run
```
The API will be available at `http://localhost:5002`.

### 3. Frontend Setup (Angular)
Open another terminal in the `FeedbackTrack.Client` directory.

```bash
cd FeedbackTrack.Client
# Install dependencies
npm install
# Run the development server
npm start
```
The application will be available at `http://localhost:4200`.

## Role-Based Access

The application features three distinct roles with specific responsibilities:

- **Employee**: Submit feedback, give recognition, and view personal appreciation.
- **Manager**: Review team feedback, monitor engagement, and generate reports.
- **Admin**: Configure system settings, manage users, and oversee analytics (Auto-login available).

## Troubleshooting

- **Database Errors**: If the database doesn't create automatically, ensure you have the `dotnet-ef` tool installed: `dotnet tool install --global dotnet-ef`.
- **Port Issues**: If ports 5002 or 4200 are in use, restart your dev environment or terminal.
