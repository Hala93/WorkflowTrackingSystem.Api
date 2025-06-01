# WorkflowTrackingSystem.Api

## Overview

This repository contains the backend API for the Workflow Management System. Developed using .NET Core / .NET 5+ Web API with Entity Framework Core, this API provides RESTful endpoints for defining, executing, and tracking business workflows and processes.
It includes a robust validation mechanism to ensure data integrity during process execution.

## Features

* **Workflow Definition:**
    * Create, update, and retrieve workflow definitions, including their sequential steps.
* **Process Execution:**
    * Initiate new process instances based on defined workflows.
    * Execute individual steps within an active process.
* **Process Tracking:**
    * Retrieve lists of active, completed, or pending processes with various filtering options (by workflow, status, assigned user).
* **Validation Middleware:**
    * Custom validation mechanism for process steps, capable of simulating external API calls for data validation before allowing a process to advance.
    * Logs validation results, including rejected attempts.

## Technologies Used

* **Framework:** .NET Core / .NET 5+
* **Web Framework:** ASP.NET Core Web API
* **ORM:** Entity Framework Core
* **Database:** (e.g., SQL Server, PostgreSQL, SQLite - *Specify your actual choice here*)
* **Serialization:** JSON
* **Logging:** Built-in .NET Core logging

## Prerequisites

Before you begin, ensure you have the following installed:

* [.NET SDK](https://dotnet.microsoft.com/download) (5.0 or higher, depending on the project's target framework)
* [Git](https://git-scm.com/downloads)
* A compatible database server (e.g., SQL Server, PostgreSQL, or SQLite for local development).

## Setup Instructions

Follow these steps to get the API up and running on your local machine.

### 1. Clone the Repository

First, clone the API repository to your local machine:

```bash
git clone <https://github.com/Hala93/WorkflowTrackingSystem.Api>
cd WorkflowTrackingSystem.Api

### To link local project with git
git init
git remote add origin https://github.com/Hala93/WorkflowTrackingSystem.Api.git
git add .
git commit -m "your message"
git push -u origin master or git push -up--stream origin master

### if the local project is linked with git
git add .
git commit -m "message"
git push

## Configure Database Connection
Open the appsettings.json and then
Update the ConnectionStrings section to point to your database.
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WorkflowTrackingDB;Trusted_Connection=True;MultipleActiveResultSets=true"

## Apply Entity Framework Core Migrations

dotnet ef database update

## if need a new migration
dotnet ef migrations add YourMigrationName
dotnet ef database update

## Run API
dotnet run

Validation Middleware
A crucial component of this API is the custom validation middleware. It intercepts requests to POST /v1/processes/execute. If a specific WorkflowStep is configured to RequiresValidation, the middleware will simulate (or in a real scenario, call) an external API to validate the provided input data or process context. If validation fails, the process step is prevented from advancing, and an error response is returned to the client, along with a log of the validation failure.

Project Structure
The API project is organized into the following key directories:

Controllers/: Contains the API controllers responsible for handling HTTP requests.
Models/: Defines the data models for entities (e.g., Workflow, WorkflowStep, Process, ProcessStep, ValidationLog).
Data/: Holds the ApplicationDbContext for Entity Framework Core, managing database interactions.
Services/: Implements the core business logic for workflow and process management.
Middleware/: Contains the custom validation middleware logic.
DTOs/: (If implemented) Data Transfer Objects for request and response serialization.

## to test the API
Postman or Swagger UI
https://localhost:44325/index.html
