using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Zealand_Eksamen.Models;

namespace Zealand_Eksamen.Models
{
    public class Exam
    {
        public int ExamID { get; set; }
        [Required(ErrorMessage = "Eksamensnavn er påkrævet")]
        public string ExamName { get; set; } = string.Empty;
        public bool HasSupervision { get; set; }
        public int? EstimatedStudents { get; set; }
        public string? CensorType { get; set; }
        public DateTime? OrdinaryDeliveryDate { get; set; }
        public DateTime? OrdinaryStartDate { get; set; }
        public DateTime? OrdinaryEndDate { get; set; }
        public DateTime? ReexamDeliveryDate { get; set; }
        public DateTime? ReexamDate { get; set; }
        
        // Foreign keys
        [Required(ErrorMessage = "Hold er påkrævet")]
        public int? ClassID { get; set; }
        public int? ExamTypeID { get; set; }

        // Navigation properties
        [ValidateNever]
        public Class Class { get; set; } = null!;
        [ValidateNever]
        public ExamType? ExamType { get; set; }
        public ICollection<ExamAssignment> ExamAssignments { get; set; } = new List<ExamAssignment>();
    }
}