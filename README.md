# Survey App

A full-stack survey application built with React.js, .NET 8, and SQLite.

## Features

- **User Authentication**: JWT-based login and registration
- **Survey Management**: Create, edit, delete, and activate/deactivate surveys
- **Question Types**: Single choice and multiple choice questions
- **Survey Taking**: Users can take active surveys and submit responses
- **Response History**: View past survey submissions with answers
- **Survey Statistics**: See response counts and percentages for each question
- **Admin Panel**: Manage users, promote/demote admins, delete users
- **Public/Private Surveys**: Browse active surveys or manage your own

## Tech Stack

### Backend
- .NET 8 Web API
- Entity Framework Core
- SQLite (development) / PostgreSQL (production ready)
- JWT Authentication
- Swagger/OpenAPI

### Frontend
- React 18 with TypeScript
- React Router for navigation
- Axios for API calls
- Material-UI (basic styling)

### Infrastructure
- Docker & Docker Compose
- Git for version control

## Prerequisites

- .NET 8 SDK
- Node.js 18+
- Docker (optional)
- Git

## Quick Start

### 1. Clone the repository

git clone https://github.com/yourusername/survey-app.git
cd survey-app

### 2. Backend Setup

cd backend/SurveyApi
dotnet restore
dotnet run

### 3. Frontend Setup

cd frontend
npm install
npm start

### 4. Database

The app uses SQLite by default - no additional setup needed. The database file survey.db will be created automatically.

### 5. Default Admin

cd backend/SurveyApi
sqlite3 survey.db "UPDATE Users SET IsAdmin = 1 WHERE Id = 1;"