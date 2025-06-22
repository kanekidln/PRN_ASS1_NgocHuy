# FU Mini Hotel System

This is a hotel management system built with .NET 6 and WPF, using Entity Framework Core for database access.

## Prerequisites

- .NET 8 SDK
- SQL Server 2022
- Visual Studio 2022 (recommended)

## Database Setup

The application uses Entity Framework Core Code First approach to create and manage the database. The database will be automatically created when you run the application for the first time.

1. Make sure you have SQL Server 2022 installed and running.
2. Check the connection string in `appsettings.json` files (located in both BusinessObjects and huy projects) and update if necessary.
3. The default connection string is: `Server=.;Database=hotel;Trusted_Connection=True;TrustServerCertificate=True;`

## Running the Application

1. Open the solution file `Huy-PRN.sln` in Visual Studio.
2. Set the `huy` project as the startup project.
3. Build the solution.
4. Run the application.

On first run, the application will:
1. Create the database if it doesn't exist
2. Create all required tables
3. Seed the database with initial data

## Login Information

The application is seeded with the following user accounts:

1. Email: john.doe@example.com
   Password: password123

2. Email: jane.smith@example.com
   Password: password456

## Features

- Room management
- Customer management
- Booking reservations
- Reporting

## Project Structure

- **BusinessObjects**: Contains the entity models and DbContext
- **DataAccessObjects**: Contains the data access layer
- **Repositories**: Contains the repository interfaces
- **Services**: Contains the business logic
- **QuangKietWPF**: Contains the WPF application UI

## How to run

1. Open file `huy.sln` by Visual Studio
2. Build solution
3. Run:
dotnet ef migrations add Create
dotnet ef database update
dotnet run

## Login information

### Admin:
- **Email**: admin@FUMiniHotelSystem.com
- **Password**: @@abc123@@
