using Microsoft.Data.SqlClient;
using EmployeeManagementSystem.Models;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagementSystem.Data;

public class EmployeeService
{
    private readonly string connectionString;

    public EmployeeService(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        var employees = new List<Employee>();
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SELECT * FROM Employees", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var emp = new Employee();
                emp.EmployeeID = reader["EmployeeID"] != DBNull.Value ? (int)reader["EmployeeID"] : 0;
                emp.FirstName = reader["FirstName"] != DBNull.Value ? reader["FirstName"].ToString() : string.Empty;
                emp.LastName = reader["LastName"] != DBNull.Value ? reader["LastName"].ToString() : string.Empty;
                emp.Salary = reader["Salary"] != DBNull.Value ? (decimal)reader["Salary"] : 0;
                emp.DepartmentID = reader["DepartmentID"] != DBNull.Value ? (int)reader["DepartmentID"] : 0;
                emp.LocationID = reader["LocationID"] != DBNull.Value ? (int)reader["LocationID"] : 0;
                emp.ManagerID = reader["ManagerID"] != DBNull.Value ? (int)reader["ManagerID"] : 0;
                employees.Add(emp);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllEmployeesAsync: {ex.Message}");
        }
        return employees;
    }

    public List<Employee> GetAllEmployees()
    {
        return GetAllEmployeesAsync().GetAwaiter().GetResult();
    }
    
    public async Task<(bool success, string message)> AddEmployeeAsync(Employee employee)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"INSERT INTO Employees (FirstName, LastName, Salary, DepartmentID, LocationID, ManagerID)
                                     VALUES (@firstName, @lastName, @salary, @deptId, @locId, @managerId)", conn);
            
            cmd.Parameters.AddWithValue("@firstName", employee.FirstName);
            cmd.Parameters.AddWithValue("@lastName", employee.LastName);
            cmd.Parameters.AddWithValue("@salary", employee.Salary);
            cmd.Parameters.AddWithValue("@deptId", employee.DepartmentID);
            cmd.Parameters.AddWithValue("@locId", employee.LocationID);
            cmd.Parameters.AddWithValue("@managerId", employee.ManagerID == 0 ? DBNull.Value : (object)employee.ManagerID);
            
            await cmd.ExecuteNonQueryAsync();
            return (true, "Employee added successfully.");
        }
        catch (SqlException ex)
        {
            return (false, $"Database error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    public void AddEmployee(Employee employee)
    {
        var result = AddEmployeeAsync(employee).GetAwaiter().GetResult();
        if (!result.success)
            throw new Exception(result.message);
    }

    public async Task<(bool success, string message)> UpdateEmployeeAsync(Employee employee)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            var cmd = new SqlCommand(@"UPDATE Employees 
                                     SET FirstName = @firstName, 
                                         LastName = @lastName,
                                         Salary = @salary,
                                         DepartmentID = @deptId,
                                         LocationID = @locId,
                                         ManagerID = @managerId
                                     WHERE EmployeeID = @id", conn);
            
            cmd.Parameters.AddWithValue("@firstName", employee.FirstName);
            cmd.Parameters.AddWithValue("@lastName", employee.LastName);
            cmd.Parameters.AddWithValue("@salary", employee.Salary);
            cmd.Parameters.AddWithValue("@deptId", employee.DepartmentID);
            cmd.Parameters.AddWithValue("@locId", employee.LocationID);
            cmd.Parameters.AddWithValue("@managerId", employee.ManagerID == 0 ? DBNull.Value : (object)employee.ManagerID);
            cmd.Parameters.AddWithValue("@id", employee.EmployeeID);
            
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
                return (true, "Employee updated successfully.");
            else
                return (false, "Employee not found.");
        }
        catch (SqlException ex)
        {
            return (false, $"Database error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    public void UpdateEmployee(Employee employee)
    {
        var result = UpdateEmployeeAsync(employee).GetAwaiter().GetResult();
        if (!result.success)
            throw new Exception(result.message);
    }

    public async Task<(bool success, string message)> DeleteEmployeeAsync(int id)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // First check if employee exists
            var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Employees WHERE EmployeeID = @id", conn);
            checkCmd.Parameters.AddWithValue("@id", id);
            var existsResult = await checkCmd.ExecuteScalarAsync();
            int exists = Convert.ToInt32(existsResult ?? 0);
            
            if (exists == 0)
                return (false, "Employee not found.");

            // Check if employee is a manager
            var managerCheckCmd = new SqlCommand("SELECT COUNT(1) FROM Employees WHERE ManagerID = @id", conn);
            managerCheckCmd.Parameters.AddWithValue("@id", id);
            var hasSubordinatesResult = await managerCheckCmd.ExecuteScalarAsync();
            int hasSubordinates = Convert.ToInt32(hasSubordinatesResult ?? 0);
            
            if (hasSubordinates > 0)
                return (false, "Cannot delete employee who is a manager. Please reassign their subordinates first.");

            var cmd = new SqlCommand("DELETE FROM Employees WHERE EmployeeID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            
            if (rowsAffected > 0)
                return (true, "Employee deleted successfully.");
            else
                return (false, "Failed to delete employee.");
        }
        catch (SqlException ex)
        {
            return (false, $"Database error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred: {ex.Message}");
        }
    }

    public void DeleteEmployee(int id)
    {
        var result = DeleteEmployeeAsync(id).GetAwaiter().GetResult();
        if (!result.success)
            throw new Exception(result.message);
    }
}
