# E-Commerce API

A RESTful E-Commerce backend API built using ASP.NET Core Web API.  
The project follows clean architecture principles and provides features for authentication, product management, orders, and role-based authorization.

## Technologies Used

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- LINQ
- Dependency Injection
- N-Tier Architecture

## Features

- User Registration & Login
- JWT Authentication & Authorization
- Role-Based Access Control (Admin / Customer)
- Product Management
- Category Management
- Shopping Cart
- Order Management
- Secure API Endpoints
- Clean Architecture Structure

## Project Structure

```bash
E-commerceApis/
│
├── Controllers
├── Services
├── Repositories
├── DTOs
├── Models
├── Data
├── Middleware
└── Helpers

## Prerequisites
.NET 8 SDK
SQL Server
Visual Studio 2022
Installation
Clone the repository

git clone https://github.com/abdulrahmanebrahim72/e-commerceApis.git

Clone the repository
git clone https://github.com/abdulrahmanebrahim72/e-commerceApis.git
Open the project in Visual Studio
Update the database connection string in appsettings.json
Apply migrations
update-database
Run the project
dotnet run
Authentication

This project uses JWT Authentication.
After login, use the generated token in the Authorization header:

Bearer YOUR_TOKEN
