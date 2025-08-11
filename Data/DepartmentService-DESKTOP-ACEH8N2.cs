using Microsoft.Data.SqlClient;
using EmployeeManagementSystem.Models;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagementSystem.Data;

public class DepartmentService
{
    private readonly string connectionString;

    public DepartmentService(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<List<Department>> GetAllDepartmentsAsync()
    {
        var departments = new List<Department>();
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SELECT DepartmentID, DepartmentName FROM Departments", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                departments.Add(new Department
                {
                    DepartmentID = reader.GetInt32(0),
                    DepartmentName = reader.GetString(1)
                });
            }
        }
        catch (Exception ex)
        {
            // Log the error in a production environment
            Console.WriteLine($"Error in GetAllDepartmentsAsync: {ex.Message}");
        }
        return departments;
    }

    public List<Department> GetAllDepartments()
    {
        return GetAllDepartmentsAsync().GetAwaiter().GetResult();
    }

    public async Task<(bool success, string message)> AddDepartmentAsync(Department department)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // Check if department name already exists
            var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Departments WHERE DepartmentName = @name", conn);
            checkCmd.Parameters.AddWithValue("@name", department.DepartmentName);
            var existsResult = await checkCmd.ExecuteScalarAsync();
            int exists = Convert.ToInt32(existsResult ?? 0);

            if (exists > 0)
                return (false, "A department with this name already exists.");

            var cmd = new SqlCommand(
                "INSERT INTO Departments (DepartmentName) VALUES (@name); SELECT SCOPE_IDENTITY();", 
                conn);
            cmd.Parameters.AddWithValue("@name", department.DepartmentName);
            
            var result = await cmd.ExecuteScalarAsync();
            if (result != null)
            {
                department.DepartmentID = Convert.ToInt32(result);
                return (true, "Department added successfully.");
            }
            return (false, "Failed to add department.");
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

    public (bool success, string message) AddDepartment(Department department)
    {
        return AddDepartmentAsync(department).GetAwaiter().GetResult();
    }

    public async Task<(bool success, string message)> DeleteDepartmentAsync(int id)
    {
        using var conn = new SqlConnection(connectionString);
        try
        {
            await conn.OpenAsync();

            using var transaction = (SqlTransaction)await conn.BeginTransactionAsync();
            try
            {
                // First check if department exists
                var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Departments WHERE DepartmentID = @id", conn, transaction);
                checkCmd.Parameters.AddWithValue("@id", id);
                var existsResult = await checkCmd.ExecuteScalarAsync();
                int exists = Convert.ToInt32(existsResult ?? 0);
                
                if (exists == 0)
                    return (false, "Department not found.");

                // Check if department has any employees
                var empCheckCmd = new SqlCommand("SELECT COUNT(1) FROM Employees WHERE DepartmentID = @id", conn, transaction);
                empCheckCmd.Parameters.AddWithValue("@id", id);
                var hasEmployeesResult = await empCheckCmd.ExecuteScalarAsync();
                int hasEmployees = Convert.ToInt32(hasEmployeesResult ?? 0);
                
                if (hasEmployees > 0)
                    return (false, "Cannot delete department that has employees. Please reassign or remove employees first.");

                // Proceed with deletion if all checks pass
                var deleteCmd = new SqlCommand("DELETE FROM Departments WHERE DepartmentID = @id", conn, transaction);
                deleteCmd.Parameters.AddWithValue("@id", id);
                int rowsAffected = await deleteCmd.ExecuteNonQueryAsync();
                
                if (rowsAffected > 0)
                {
                    await transaction.CommitAsync();
                    return (true, "Department deleted successfully.");
                }
                else
                {
                    await transaction.RollbackAsync();
                    return (false, "Failed to delete department.");
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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

    public (bool success, string message) DeleteDepartment(int id)
    {
        return DeleteDepartmentAsync(id).GetAwaiter().GetResult();
    }

    public async Task<(bool success, string message)> UpdateDepartmentAsync(Department department)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // Check if department exists
            var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Departments WHERE DepartmentID = @id", conn);
            checkCmd.Parameters.AddWithValue("@id", department.DepartmentID);
            var existsResult = await checkCmd.ExecuteScalarAsync();
            int exists = Convert.ToInt32(existsResult ?? 0);

            if (exists == 0)
                return (false, "Department not found.");

            // Check if new name conflicts with existing department
            var nameCheckCmd = new SqlCommand(
                "SELECT COUNT(1) FROM Departments WHERE DepartmentName = @name AND DepartmentID != @id", 
                conn);
            nameCheckCmd.Parameters.AddWithValue("@name", department.DepartmentName);
            nameCheckCmd.Parameters.AddWithValue("@id", department.DepartmentID);
            var nameExistsResult = await nameCheckCmd.ExecuteScalarAsync();
            int nameExists = Convert.ToInt32(nameExistsResult ?? 0);

            if (nameExists > 0)
                return (false, "A department with this name already exists.");

            var updateCmd = new SqlCommand(
                "UPDATE Departments SET DepartmentName = @name WHERE DepartmentID = @id",
                conn);
            updateCmd.Parameters.AddWithValue("@name", department.DepartmentName);
            updateCmd.Parameters.AddWithValue("@id", department.DepartmentID);
            
            int rowsAffected = await updateCmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
                return (true, "Department updated successfully.");
            else
                return (false, "Failed to update department.");
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

    public (bool success, string message) UpdateDepartment(Department department)
    {
        return UpdateDepartmentAsync(department).GetAwaiter().GetResult();
    }
}