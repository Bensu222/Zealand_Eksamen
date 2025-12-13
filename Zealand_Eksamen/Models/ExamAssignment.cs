using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Zealand_Eksamen.Models
{
    public class ExamAssignment
    {
        public int ExamAssignmentID { get; set; }
        public int ExamID { get; set; }
        public int EmployeeID { get; set; }
        public string? RoleInExam { get; set; }

        // Navigation properties
        [ValidateNever]
        public Exam Exam { get; set; } = null!;
        [ValidateNever]
        public Employee Employee { get; set; } = null!;
    }
}