using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models;

public class Location
{
    public int LocationID { get; set; }
    
    [Required(ErrorMessage = "City is required")]
    [StringLength(50, ErrorMessage = "City cannot be longer than 50 characters")]
    public string City { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Country is required")]
    [StringLength(50, ErrorMessage = "Country cannot be longer than 50 characters")]
    public string Country { get; set; } = string.Empty;
}
