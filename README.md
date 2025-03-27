# Developer Evaluation Project - Ambev

This project is part of the technical challenge for a developer position at Ambev Tech.

## Tech Stack

- .NET 8
- MediatR
- FluentValidation
- PostgreSQL (via Docker)
- Entity Framework Core
- xUnit + NSubstitute (Testing)
- Swagger for API documentation

## Implemented Features

- Full CRUD for Sales
- Cancel a specific item within a Sale
- Quantity-based discount business rules
- FluentValidation for request validation
- Simulated events (logged using `ILogger`):
  - SaleCreated
  - SaleModified
  - SaleDeleted
  - ItemCancelled
- Swagger API documentation
- Unit tests covering key business scenarios

## How to Run the Project

### 1. Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Git

### 2. Start the Services (PostgreSQL, Redis, Mongo)

bash
docker-compose up -d
This will start all required services for the API.

3. Apply the Migrations
dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM --startup-project src/Ambev.DeveloperEvaluation.WebApi

4. Run the API
cd src/Ambev.DeveloperEvaluation.WebApi
dotnet run

API will be available at:
http://localhost:8080/swagger

Running Unit Tests
dotnet test
This will run all unit tests using xUnit.

Business Rules
Orders with 4 or more identical items: 10% discount

Orders with 10 to 20 identical items: 20% discount

Maximum allowed: 20 items per product

Orders with less than 4 items: no discount

Project Structure
src/ - Main application source

WebApi - API Layer

Application - Business logic and handlers

Domain - Entities and core domain

ORM - Database configuration (EF Core)

IoC - Dependency injection

tests/ - Unit test projects

Notes
Event publishing is simulated using ILogger, as suggested.

All domain logic is handled by MediatR handlers.

Swagger provides complete route documentation.
