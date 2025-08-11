# Employee Management System

A comprehensive Employee Management System built with ASP.NET Core 7.0 and Blazor Server. This application provides full CRUD (Create, Read, Update, Delete) operations for managing employees, departments, and locations.

## Features

- **Employee Management**: Add, edit, view, and delete employee records
- **Department Management**: Manage company departments with validation
- **Location Management**: Handle company locations and office sites
- **Data Validation**: Built-in input validation and error handling
- **Responsive Design**: Bootstrap-based UI that works on all devices
- **Real-time Updates**: Blazor Server for interactive user interface

## Technologies Used

- **Backend**: ASP.NET Core 7.0
- **Frontend**: Blazor Server
- **Database**: SQL Server with Entity Framework Core
- **UI Framework**: Bootstrap 5
- **Icons**: Open Iconic
- **Data Access**: ADO.NET with SqlClient

## Project Structure

```
EmployeeManagementSystem/
├── Data/
│   ├── DepartmentService.cs    # Department CRUD operations
│   ├── EmployeeService.cs      # Employee CRUD operations
│   └── LocationService.cs      # Location CRUD operations
├── Models/
│   ├── Department.cs           # Department entity
│   ├── Employee.cs             # Employee entity
│   └── Location.cs             # Location entity
├── Pages/
│   ├── Departments.razor       # Department management page
│   ├── Employees.razor         # Employee management page
│   ├── Locations.razor         # Location management page
│   └── Index.razor             # Home page
├── Shared/
│   ├── MainLayout.razor        # Main layout component
│   ├── NavMenu.razor           # Navigation menu
│   └── InputValidation.razor   # Input validation component
└── wwwroot/                    # Static files (CSS, JS, images)
```

## Getting Started

### Prerequisites

- .NET 7.0 SDK or later
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/MishalSaleem/EmployeeManagementSystem_.git
   cd EmployeeManagementSystem_
   ```

2. Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeManagementDB;Trusted_Connection=true;"
     }
   }
   ```

3. Create the database and tables:
   ```sql
   CREATE DATABASE EmployeeManagementDB;
   
   USE EmployeeManagementDB;
   
   CREATE TABLE Departments (
       DepartmentID int IDENTITY(1,1) PRIMARY KEY,
       DepartmentName nvarchar(100) NOT NULL UNIQUE
   );
   
   CREATE TABLE Locations (
       LocationID int IDENTITY(1,1) PRIMARY KEY,
       City nvarchar(100) NOT NULL,
       Country nvarchar(100) NOT NULL
   );
   
   CREATE TABLE Employees (
       EmployeeID int IDENTITY(1,1) PRIMARY KEY,
       FirstName nvarchar(50) NOT NULL,
       LastName nvarchar(50) NOT NULL,
       Email nvarchar(100) UNIQUE,
       Phone nvarchar(20),
       HireDate datetime NOT NULL,
       JobTitle nvarchar(100),
       Salary decimal(10,2),
       DepartmentID int FOREIGN KEY REFERENCES Departments(DepartmentID),
       LocationID int FOREIGN KEY REFERENCES Locations(LocationID)
   );
   ```

4. Build and run the application:
   ```bash
   dotnet build
   dotnet run
   ```

5. Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`

## Usage

### Managing Departments
- Navigate to the "Departments" page
- Add new departments with unique names
- Edit existing department information
- Delete departments (only if no employees are assigned)

### Managing Locations
- Go to the "Locations" page
- Add company locations with city and country
- Update location details as needed
- Remove unused locations

### Managing Employees
- Access the "Employees" page
- Create new employee records with complete information
- Assign employees to departments and locations
- Update employee details including salary and job title
- Remove employee records when needed

## Features in Detail

### Data Validation
- Required field validation
- Email format validation
- Unique constraint handling
- Foreign key relationship validation

### Error Handling
- Database connection error handling
- SQL exception management
- User-friendly error messages
- Transaction rollback on failures

### Security Features
- SQL injection prevention using parameterized queries
- Input sanitization
- Transaction-based operations for data consistency

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

Mishal Saleem - [@MishalSaleem](https://github.com/MishalSaleem)

Project Link: [https://github.com/MishalSaleem/EmployeeManagementSystem_](https://github.com/MishalSaleem/EmployeeManagementSystem_)

## Acknowledgments

- ASP.NET Core Team for the excellent framework
- Bootstrap team for the responsive CSS framework
- Open Iconic for the icon library