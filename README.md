# SchoolHub API

A backend API built using **.NET Core Web API** to manage **students, teachers, courses, classes, attendance, and grading** with **role‑based authentication**, clean architecture principles, and relational database design.

---

## 🚀 Tech Stack

- **.NET 9 Web API** – Core backend framework.
- **Entity Framework Core** – ORM for database operations.
- **ASP.NET Core Identity** – Authentication & authorization.
- **JWT Authentication** – Role-based access control.
- **Global Exception Handling Middleware** – Unified API error responses.
- **NLog** – Centralized logging provider.
- **FluentValidation** – Request validation.
- **AutoMapper** – DTO mapping.
- **Swagger / OpenAPI** – API documentation.
---

## 📁 Project Structure

```
SchoolHub/
├── SchoolHub.Presentation      # Controllers, endpoints, filters, exception handling
├── SchoolHub.Contracts         # DTOs, request/response models
├── SchoolHub.Entities          # Domain entities & enums
├── SchoolHub.LoggerService     # NLog-based logging abstraction
├── SchoolHub.Repository        # Data access layer (EF Core repositories)
├── SchoolHub.Service           # Business logic implementations
├── SchoolHub.Service.Contracts # Interfaces for service layer
└── SchoolHub.Shared            # Shared helpers, constants, utilities
```



Uses **Clean/Onion architecture** for maintainability and testability.

---

## ⚙️ Setup Instructions

### 1️⃣ Clone the Repository
```

git clone https://github.com/Mossad-55/SchoolHub-API.git
cd SchoolHub-API

```

### 2️⃣ Configure Your Database
Update the connection string in:
```

SchoolHubApi/appsettings.json

````
Example:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=SchoolHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
````

### 3️⃣ Apply Migrations

```
dotnet ef database update
```

### 4️⃣ Run the Application

```
dotnet run --project SchoolHub.API
```

API will be available at:

```
https://localhost:5001
```

Swagger UI:

```
https://localhost:5001/swagger
```

---

## 🔑 Authentication & Roles

The API uses **JWT tokens**.
Roles:

- **Admin** – Full access
- **Teacher** – Manage classes, attendance, grading
- **Student** – View grades, courses, profile

Login via `/auth/login` to obtain a token.

Attach the token in headers:

```
Authorization: Bearer your_token_here
```

---

## 📘 How to Use the Repository

- Explore the **Controllers** folder for available endpoints.
- Review the **Domain** layer to understand the relational model.
- Use the **Application** layer to add or modify business logic.
- Use **Infrastructure** for any database or external integrations.
- Check **Swagger UI** for live API testing.

Happy coding! 🎓