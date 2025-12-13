using Zealand_Eksamen.Models;

namespace Zealand_Eksamen.Models
{
    public class ExamType
    {
        public int ExamTypeID { get; set; }
        public string TypeName { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}