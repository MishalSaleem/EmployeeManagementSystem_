using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models;

public class Employee
{
    public int EmployeeID { get; set; }
    
    [Required(ErrorMessage = "First Name is required")]
    [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
    public string LastName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Salary is required")]
    [Range(0, 1000000, ErrorMessage = "Salary must be between 0 and 1,000,000")]
    public decimal Salary { get; set; }
    
    [Required(ErrorMessage = "Department is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a department")]
    public int DepartmentID { get; set; }
    
    [Required(ErrorMessage = "Location is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a location")]
    public int LocationID { get; set; }
    
    // Manager ID is optional
    public int ManagerID { get; set; }
}
