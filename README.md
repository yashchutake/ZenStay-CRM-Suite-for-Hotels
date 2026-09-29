# Zen Hotel Management System

A comprehensive, enterprise-grade hotel management solution designed to streamline hotel operations through efficient management of bookings, rooms, employees, customers, and transportation services.

Credentials to Login
1. As Admin (Default):
   
   Username: Admin
   
   Password: Secret@123

   Once logged in, an Admin can create more Admin as well as Receptionist accounts.

3. As Receptionist
   
   Username: Receptionist
   
   Password: Secret@123

Upon clone, there may be an error regarding NuGet Packages, run ```dotnet restore``` in Package Manager Console and it should be resolved.


## 🏨 Features

- **Room Management** - Complete room inventory with real-time availability tracking
- **Booking System** - Full reservation lifecycle management with payment processing
- **Customer Management** - Comprehensive guest profiles and history tracking
- **Employee Management** - Staff directory with role-based organization
- **Transportation Services** - Cab booking and driver management
- **Dashboard & Analytics** - Real-time metrics and business intelligence

## 🏗️ Architecture

```
┌─────────────────────┐    ┌─────────────────────┐
│   Web Application   │    │     Web API         │
│   (Presentation)    │◄──►│   (Business Logic)  │
└─────────────────────┘    └─────────────────────┘
                                       │
                           ┌─────────────────────┐
                           │     Database        │
                           │   (Data Storage)    │
                           └─────────────────────┘
```

### Project Structure

```
├── Zen_HotelManagement_WebApi_Project/
│   ├── ZenHotelManagement.WebApi/          # API entry point
│   ├── ZenHotelManagement.Contracts/       # Interface definitions
│   ├── ZenHotelManagement.Entities/        # Domain models
│   ├── ZenHotelManagement.LoggerService/   # Logging infrastructure
│   ├── ZenHotelManagement.Presentation/    # API controllers
│   ├── ZenHotelManagement.Repository/      # Data access layer
│   ├── ZenHotelManagement.Service/         # Business logic layer
│   ├── ZenHotelManagement.Service.Contracts/ # Service interfaces
│   └── ZenHotelManagement.Shared/          # Shared DTOs and utilities
│
└── Zen_HotelManagement_WebApplication_Project/
    └── ZenHotelManagement.WebApp/
        ├── Controllers/                   # MVC controllers
        ├── Views/                         # Razor views
        ├── Models/                        # View models
        ├── Services/                      # HTTP client services
        ├── wwwroot/                       # Static files
        └── Program.cs                     # Application entry point
```

## 🛠️ Technology Stack

### Backend
- **.NET 8.0** - Latest LTS framework
- **ASP.NET Core Web API** - RESTful service development
- **Entity Framework Core** - ORM for database operations
- **AutoMapper** - Object mapping
- **NLog** - Logging framework

### Frontend
- **ASP.NET Core MVC** - Web application framework
- **Razor Pages** - Server-side rendering
- **Bootstrap** - Responsive UI framework
- **jQuery** - Client-side scripting

### Database
- **SQL Server** - Enterprise-grade relational database
- **Entity Framework Migrations** - Database versioning

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later
- [SQL Server 2019](https://www.microsoft.com/en-us/sql-server) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended)

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/zen-hotel-management.git
cd zen-hotel-management
```

### 2. Configure Database Connection

Update the connection string in `appsettings.json` (API Project):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ZenHotelDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 3. Run Database Migrations

```bash
cd Zen_HotelManagement_WebApi_Project/ZenHotelManagement.WebApi
dotnet ef database update
```

### 4. Start the Applications

**Terminal 1 - API:**
```bash
cd Zen_HotelManagement_WebApi_Project/ZenHotelManagement.WebApi
dotnet run
```

**Terminal 2 - Web App:**
```bash
cd Zen_HotelManagement_WebApplication_Project/ZenHotelManagement.WebApp
dotnet run
```

### 5. Access the Application

- **Web Application:** `https://localhost:7007`
- **API Documentation:** `https://localhost:5001/swagger`

## 📊 Database Schema

### Core Entities

| Table | Description |
|-------|-------------|
| **Rooms** | Room inventory with types, pricing, and status |
| **Bookings** | Reservation management with payment tracking |
| **Customer** | Guest profiles and contact information |
| **Employee** | Staff directory with role management |
| **CabDriver** | Transportation service providers |
| **CabBookings** | Transportation reservations |

### Entity Relationships

- Customer ↔ RoomBookings (One-to-Many)
- Room ↔ RoomBookings (One-to-Many)
- Customer ↔ CabBookings (One-to-Many)
- CabDriver ↔ CabBookings (One-to-Many)

## 🔌 API Endpoints

### Room Management
```http
GET    /api/rooms                   # Get all rooms
GET    /api/rooms/{roomNo}          # Get room by number
POST   /api/rooms                   # Create new room
PUT    /api/rooms/{roomNo}          # Update room
DELETE /api/rooms/{roomNo}          # Delete room
```

### Booking Management
```http
GET    /api/roombookings           # Get all bookings
GET    /api/roombookings/active    # Get active bookings
POST   /api/roombookings           # Create booking
PUT    /api/roombookings/{id}      # Update booking
```

### Customer Management
```http
GET    /api/customers              # Get all customers
GET    /api/customers/{id}         # Get customer by ID
POST   /api/customers              # Create customer
PUT    /api/customers/{id}         # Update customer
```

## 🔧 Configuration

### API Configuration (appsettings.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ZenHotelDB;Trusted_Connection=true"
  },
  "AllowedHosts": "*"
}
```

### Web App Configuration (appsettings.json)
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:5001/api"
  },
  "ConnectionStrings": {
    "ZenIdentityConnection": "Server=localhost;Database=ZenIdentityDB;Trusted_Connection=true"
  }
}
```

## 🔒 Security Features

- **Input Validation** - Comprehensive input sanitization
- **SQL Injection Prevention** - Parameterized queries with Entity Framework
- **Authentication** - ASP.NET Core Identity integration
- **Authorization** - Role-based access control
- **Data Protection** - Encrypted sensitive information

## 📈 Performance Features

- **Database Optimization** - Indexed queries and efficient data retrieval
- **Async Operations** - Non-blocking operations throughout the application


**Version:** 1.0  
**Last Updated:** June 2025  
**Maintained By:** Development Team
