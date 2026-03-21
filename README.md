# Discount Management System

A robust, enterprise-grade promotional engine built with **ASP.NET Core (MVC & Web API)**. This system manages the full lifecycle of e-commerce coupons—from merchant creation and administrative approval to customer booking and automated expiration via background services.

## 🚀 Key Features

* **Role-Based Access Control (RBAC):** Distinct workflows for **Administrators** (moderation), **Merchants** (offer management), and **Customers** (purchasing).
* **Automated Background Worker:** A dedicated `Worker Service` that handles real-time reservation cleanup and expires offers automatically.
* **Clean Architecture:** Implemented using the **Repository Pattern** and **Dependency Injection** for high maintainability.
* **Secure Transactions:** Integrated **Identity Framework** and **JWT** for secure authentication and authorization.
* **Data Integrity:** Utilizes **Fluent Validation** for strict server-side rules and **Global Exception Middleware** for standardized error handling.

## 🛠️ Technical Stack

* **Backend:** .NET 8, C#, ASP.NET Core Web API & MVC.
* **Database:** SQL Server / PostgreSQL with **Entity Framework Core (Code First)**.
* **Tools:** AutoMapper/Mapster, Swagger UI, Serilog, Health Checks.
* **Testing:** Unit Testing for the Application Layer.

## 🏗️ System Architecture

The project follows **SOLID principles** and is organized into a multi-layer solution:
1.  **Presentation:** MVC for the management portal and Web API for client interactions.
2.  **Application:** Core business logic, DTOs, and Service interfaces.
3.  **Infrastructure/Data:** EF Core DbContext and Repository implementations.

## 🚦 Getting Started

1.  **Clone the repo:** `git clone https://github.com/ujera/discount-management.git`
2.  **Update Connection String:** Set your database credentials in `appsettings.json`.
3.  **Run Migrations:** `dotnet ef database update`
4.  **Launch:** Press `F5` in Visual Studio or run `dotnet run`.

## 📷 Screenshots
*(Tip: Add a screenshot of your Swagger UI or the Merchant Dashboard here)*
