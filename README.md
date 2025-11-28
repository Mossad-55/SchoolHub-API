---
# SchoolHub‑API

A backend API built with .NET Web API to manage Students, Teachers, Courses, Batches, Departments, Attendance, Grading — with role‑based authentication, clean architecture, and relational database design.

---

## 📚 Overview

SchoolHub‑API serves as the backend for a school management system — supporting multiple roles (Admin, Teacher, Student), structured layers (repositories, services, presentation), and common educational workflows such as:

- Course & batch management
- User management (registration, login, roles)
- Soft‑delete / activate / deactivate for entities (users, courses, etc.)
- Role-based access to endpoints (Admin, Teacher, Student)
- Logging, validation, and error handling for robustness

This API can be paired with any frontend (web, mobile) to build a full-fledged school management application.

---

## 🧰 Tech Stack

- **.NET 9 Web API** – core backend framework
- **Entity Framework Core** – ORM for database operations
- **ASP.NET Core Identity** – authentication & authorization (roles)
- **JWT Authentication** – stateless authentication using tokens
- **AutoMapper** – mapping between entities and DTOs
- **FluentValidation** – validating incoming requests
- **Global Exception Handling Middleware** – consistent error responses
- **NLog / Custom Logger Service** – centralized logging
- **Swagger / OpenAPI** – API documentation & testing
- **Clean / Onion Architecture** – for maintainability and testability

---

## 🏗️ Project Structure

```
SchoolHub‑API/
├── SchoolHub.Presentation       # Controllers, endpoints, filters, exception handling
├── SchoolHub.Contracts          # DTOs, request/response models
├── SchoolHub.Entities           # Domain entities & enums
├── SchoolHub.LoggerService      # Logging abstraction (NLog or custom)
├── SchoolHub.Repository         # Data access layer (EF Core repositories)
├── SchoolHub.Service            # Business logic implementations
├── SchoolHub.Service.Contracts  # Interfaces for service layer
└── SchoolHub.Shared             # Shared helpers, constants, utilities
```

Follows Clean/Onion architecture for clear separation of concerns, easier testing, and maintainability.

---

## ⚙️ Setup & Running Locally

1. **Clone the repository**

```bash
git clone https://github.com/Mossad-55/SchoolHub-API.git
cd SchoolHub-API
```

2. **Configure the database connection**  
   Update `appsettings.json` with your SQL Server / database connection settings:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=SchoolHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. **Run EF Core migrations to create the database schema**

```bash
dotnet ef database update
```

4. **Run the application**

```bash
dotnet run --project SchoolHub.API
```

5. **Access the API**  
   Base URL (default): `https://localhost:5001`  
   Swagger UI: `https://localhost:5001/swagger` — for interactive documentation and testing

---

## 🔐 Authentication & Roles

- Authentication is done using JWT tokens.
- Default roles: `Admin`, `Teacher`, `Student`.
- Include the token in request header:

```
Authorization: Bearer <your_token>
```

- Role-based authorization ensures only authorized roles can access endpoints.

---

## ✅ Features & Functionality

- User registration, login, role assignment (Admin / Teacher / Student)
- CRUD for Courses, Batches (with soft‑delete support)
- Role‑based authorization (Admin, Teacher, Student) throughout controllers
- Soft‑delete and activation/deactivation for entities (e.g., Users, Courses)
- Pagination, filtering (search, sort) for listing endpoints
- DTO mapping with AutoMapper for clean data transfer
- Validation of request payloads with FluentValidation
- Centralized logging & error handling
- Swagger/OpenAPI for API documentation

---

## 📝 Sample API Requests (Postman Examples)

### 1. Login (to get JWT token)

**POST** `/api/auth/login`

Request Body:

```json
{
  "email": "admin@example.com",
  "password": "Admin123!"
}
```

Response:

```json
{
  "accessToken": "<jwt_token_here>",
  "refreshToken": "<refresh_token_here>",
  "expiresAt": "2025-12-01T10:00:00Z",
  "role": "Admin",
  "userId": "guid-here",
  "email": "admin@example.com"
}
```

---

### 2. Register a New User (Admin only)

**POST** `/api/auth/register`

Request Body:

```json
{
  "email": "teacher1@example.com",
  "password": "Teacher123!",
  "name": "John Doe",
  "role": "Teacher"
}
```

Response:

```json
{
  "succeeded": true,
  "errors": []
}
```

---

### 3. Get All Courses (Active only)

**GET** `/api/departments/{departmentId}/courses`

Headers:

```
Authorization: Bearer <jwt_token_here>
```

Response:

```json
[
  {
    "id": "course-guid",
    "name": "Mathematics 101",
    "isActive": true,
    "departmentId": "department-guid"
  }
]
```

---

### 4. Create a Batch (Teacher / Admin)

**POST** `/api/courses/{courseId}/batches`

Request Body:

```json
{
  "name": "Batch A",
  "startDate": "2025-12-01",
  "endDate": "2026-06-30"
}
```

Response:

```json
{
  "id": "batch-guid",
  "name": "Batch A",
  "courseId": "course-guid",
  "isActive": true
}
```

---

### 5. Soft Delete a Course (Admin only)

**PUT** `/api/departments/{departmentId}/courses/{courseId}`

Headers:

```
Authorization: Bearer <jwt_token_here>
```

Response:

```json
204 No Content
```

- Marks course as `IsActive = false`.
- Admin and Head of Department receive notifications automatically.

---

### 6. Activate Course

**PUT** `/api/departments/{departmentId}/courses/{courseId}`

---

## 🧑‍💻 How To Contribute

- Explore **Controllers** to see endpoints
- Extend **Service** + **Repository** layer for new features
- Use **DTOs** for request/response consistency
- Open Issues or Pull Requests for improvements or bug fixes

---

## 📄 API Documentation

- Swagger UI available at `https://localhost:44353/swagger`
- All endpoints documented and testable
- Use JWT token to access secured endpoints

---

## 🎯 Why This Project Is Interview‑Ready

- Demonstrates **modern .NET backend skills** (Identity, JWT, EF Core, Clean Architecture)
- Covers **soft-delete patterns**, **role-based authorization**, **validation**, **logging**
- Showcases **layered design** with repositories, services, controllers
- Ready to extend for a full school management system

---

## 🗣️ Contact / Maintainer

For questions or collaboration — open an issue or PR.

Happy coding! 🚀

---
