using Microsoft.Data.SqlClient;
using EmployeeManagementSystem.Models;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagementSystem.Data;

public class LocationService
{
    private readonly string connectionString;

    public LocationService(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<List<Location>> GetAllLocationsAsync()
    {
        var list = new List<Location>();
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SELECT * FROM Locations", conn);
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                var location = new Location();
                location.LocationID = reader["LocationID"] != DBNull.Value ? (int)reader["LocationID"] : 0;
                location.City = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty;
                location.Country = reader["Country"] != DBNull.Value ? reader["Country"].ToString() : string.Empty;
                list.Add(location);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAllLocationsAsync: {ex.Message}");
        }
        return list;
    }

    public List<Location> GetAllLocations()
    {
        return GetAllLocationsAsync().GetAwaiter().GetResult();
    }

    public async Task<(bool success, string message)> AddLocationAsync(Location location)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // Check if location already exists
            var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Locations WHERE City = @city AND Country = @country", conn);
            checkCmd.Parameters.AddWithValue("@city", location.City);
            checkCmd.Parameters.AddWithValue("@country", location.Country);
            var existsResult = await checkCmd.ExecuteScalarAsync();
            int exists = Convert.ToInt32(existsResult ?? 0);

            if (exists > 0)
                return (false, "A location with this city and country already exists.");

            var cmd = new SqlCommand("INSERT INTO Locations (City, Country) VALUES (@city, @country)", conn);
            cmd.Parameters.AddWithValue("@city", location.City);
            cmd.Parameters.AddWithValue("@country", location.Country);
            await cmd.ExecuteNonQueryAsync();
            return (true, "Location added successfully.");
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

    public void AddLocation(Location location)
    {
        var result = AddLocationAsync(location).GetAwaiter().GetResult();
        if (!result.success)
            throw new Exception(result.message);
    }

    public async Task<(bool success, string message)> DeleteLocationAsync(int id)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // Check if location exists
            var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Locations WHERE LocationID = @id", conn);
            checkCmd.Parameters.AddWithValue("@id", id);
            var existsResult = await checkCmd.ExecuteScalarAsync();
            int exists = Convert.ToInt32(existsResult ?? 0);
            
            if (exists == 0)
                return (false, "Location not found.");

            // Check if location has any employees
            var empCheckCmd = new SqlCommand("SELECT COUNT(1) FROM Employees WHERE LocationID = @id", conn);
            empCheckCmd.Parameters.AddWithValue("@id", id);
            var hasEmployeesResult = await empCheckCmd.ExecuteScalarAsync();
            int hasEmployees = Convert.ToInt32(hasEmployeesResult ?? 0);
            
            if (hasEmployees > 0)
                return (false, "Cannot delete location that has employees. Please reassign or remove employees first.");

            var cmd = new SqlCommand("DELETE FROM Locations WHERE LocationID = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            
            if (rowsAffected > 0)
                return (true, "Location deleted successfully.");
            else
                return (false, "Failed to delete location.");
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

    public void DeleteLocation(int id)
    {
        var result = DeleteLocationAsync(id).GetAwaiter().GetResult();
        if (!result.success)
            throw new Exception(result.message);
    }

    public async Task<(bool success, string message)> UpdateLocationAsync(Location location)
    {
        try
        {
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();

            // Check if location exists
            var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Locations WHERE LocationID = @id", conn);
            checkCmd.Parameters.AddWithValue("@id", location.LocationID);
            var existsResult = await checkCmd.ExecuteScalarAsync();
            int exists = Convert.ToInt32(existsResult ?? 0);

            if (exists == 0)
                return (false, "Location not found.");

            // Check if new city/country combination conflicts with existing location
            var nameCheckCmd = new SqlCommand(
                "SELECT COUNT(1) FROM Locations WHERE City = @city AND Country = @country AND LocationID != @id", 
                conn);
            nameCheckCmd.Parameters.AddWithValue("@city", location.City);
            nameCheckCmd.Parameters.AddWithValue("@country", location.Country);
            nameCheckCmd.Parameters.AddWithValue("@id", location.LocationID);
            var nameExistsResult = await nameCheckCmd.ExecuteScalarAsync();
            int nameExists = Convert.ToInt32(nameExistsResult ?? 0);

            if (nameExists > 0)
                return (false, "A location with this city and country already exists.");

            var cmd = new SqlCommand("UPDATE Locations SET City = @city, Country = @country WHERE LocationID = @id", conn);
            cmd.Parameters.AddWithValue("@city", location.City);
            cmd.Parameters.AddWithValue("@country", location.Country);
            cmd.Parameters.AddWithValue("@id", location.LocationID);
            
            int rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
                return (true, "Location updated successfully.");
            else
                return (false, "Failed to update location.");
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

    public void UpdateLocation(Location location)
    {
        var result = UpdateLocationAsync(location).GetAwaiter().GetResult();
        if (!result.success)
            throw new Exception(result.message);
    }
}
