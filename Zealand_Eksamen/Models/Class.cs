namespace Zealand_Eksamen.Models
{
    public class Class
    {
        public int ClassID { get; set; }
        public string ClassCode { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Programme { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}