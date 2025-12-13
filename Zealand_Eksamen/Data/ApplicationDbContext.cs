using Task = System.Threading.Tasks.Task;

using Microsoft.EntityFrameworkCore;
using Zealand_Eksamen.Models;

namespace Zealand_Eksamen.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ExamType> ExamTypes { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamAssignment> ExamAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map to existing singular table names from Database/Tabels.sql
            modelBuilder.Entity<Employee>().ToTable("Employee");
            modelBuilder.Entity<Class>().ToTable("Class");
            modelBuilder.Entity<ExamType>().ToTable("ExamType");
            modelBuilder.Entity<Exam>().ToTable("Exam");
            modelBuilder.Entity<ExamAssignment>().ToTable("ExamAssignment");

            // Unique constraints matching the SQL script
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<Class>()
                .HasIndex(c => c.ClassCode)
                .IsUnique();

            modelBuilder.Entity<ExamType>()
                .HasIndex(t => t.TypeName)
                .IsUnique();

            modelBuilder.Entity<ExamAssignment>()
                .HasIndex(ea => new { ea.ExamID, ea.EmployeeID })
                .IsUnique();

            // Relationships (align with FK names; cascade delete on ExamAssignment FKs as in SQL)
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.Class)
                .WithMany(c => c.Exams)
                .HasForeignKey(e => e.ClassID);

            modelBuilder.Entity<Exam>()
                .HasOne(e => e.ExamType)
                .WithMany(t => t.Exams)
                .HasForeignKey(e => e.ExamTypeID);

            modelBuilder.Entity<ExamAssignment>()
                .HasOne(ea => ea.Exam)
                .WithMany(e => e.ExamAssignments)
                .HasForeignKey(ea => ea.ExamID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamAssignment>()
                .HasOne(ea => ea.Employee)
                .WithMany(emp => emp.ExamAssignments)
                .HasForeignKey(ea => ea.EmployeeID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
