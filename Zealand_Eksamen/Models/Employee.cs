namespace Zealand_Eksamen.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Department { get; set; }

        // Navigation property
        public ICollection<ExamAssignment> ExamAssignments { get; set; } = new List<ExamAssignment>();
    }
}