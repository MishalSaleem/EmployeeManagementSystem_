using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models;

public class Department
{
    public int DepartmentID { get; set; }
    

    [Required(ErrorMessage = "Department Name is required")]
    [StringLength(50, ErrorMessage = "Department Name cannot be longer than 50 characters")]
    public string DepartmentName { get; set; } = string.Empty;
}
