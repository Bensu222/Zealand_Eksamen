using Microsoft.AspNetCore.Mvc;
using Zealand_Eksamen.Controllers;
using Zealand_Eksamen.Models;
using Zealand_Eksamen.Tests.TestUtilities;

namespace Zealand_Eksamen.Tests.Controllers;

public class ExamAssignmentsControllerTests
{
    [Fact]
    public async Task PostExamAssignment_ReturnsConflict_WhenDuplicateExists()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();

        using (var setupContext = TestDbContextFactory.CreateContext(connection))
        {
            var employee = new Employee { EmployeeID = 1, FullName = "John Doe", Email = "john@example.com", Role = "Teacher" };
            var classEntity = new Class { ClassID = 1, ClassCode = "CLS1", Semester = "Fall", Programme = "CS" };
            var examType = new ExamType { ExamTypeID = 1, TypeName = "Written" };
            var exam = new Exam
            {
                ExamID = 1,
                ExamName = "Exam 1",
                ClassID = classEntity.ClassID,
                Class = classEntity,
                ExamTypeID = examType.ExamTypeID,
                ExamType = examType
            };

            setupContext.AddRange(employee, classEntity, examType, exam);
            setupContext.ExamAssignments.Add(new ExamAssignment
            {
                ExamAssignmentID = 1,
                ExamID = exam.ExamID,
                EmployeeID = employee.EmployeeID
            });

            await setupContext.SaveChangesAsync();
        }

        using (var testContext = TestDbContextFactory.CreateContext(connection))
        {
            var controller = new ExamAssignmentsController(testContext);
            var duplicate = new ExamAssignment
            {
                ExamID = 1,
                EmployeeID = 1
            };

            var result = await controller.PostExamAssignment(duplicate);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal("An assignment already exists for this exam and employee combination.", conflictResult.Value);
        }
    }

    [Fact]
    public async Task PutExamAssignment_ReturnsConflict_WhenUpdatingToExistingCombination()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();

        using (var setupContext = TestDbContextFactory.CreateContext(connection))
        {
            var classEntity = new Class { ClassID = 1, ClassCode = "CLS1", Semester = "Fall", Programme = "CS" };
            var examType = new ExamType { ExamTypeID = 1, TypeName = "Written" };
            var exam = new Exam
            {
                ExamID = 1,
                ExamName = "Exam 1",
                ClassID = classEntity.ClassID,
                Class = classEntity,
                ExamTypeID = examType.ExamTypeID,
                ExamType = examType
            };

            var employeeA = new Employee { EmployeeID = 1, FullName = "John Doe", Email = "john@example.com", Role = "Teacher" };
            var employeeB = new Employee { EmployeeID = 2, FullName = "Jane Doe", Email = "jane@example.com", Role = "Assistant" };

            setupContext.AddRange(classEntity, examType, exam, employeeA, employeeB);

            setupContext.ExamAssignments.AddRange(
                new ExamAssignment
                {
                    ExamAssignmentID = 1,
                    ExamID = exam.ExamID,
                    EmployeeID = employeeA.EmployeeID
                },
                new ExamAssignment
                {
                    ExamAssignmentID = 2,
                    ExamID = exam.ExamID,
                    EmployeeID = employeeB.EmployeeID
                });

            await setupContext.SaveChangesAsync();
        }

        using (var testContext = TestDbContextFactory.CreateContext(connection))
        {
            var controller = new ExamAssignmentsController(testContext);
            var update = new ExamAssignment
            {
                ExamAssignmentID = 2,
                ExamID = 1,
                EmployeeID = 1
            };

            var result = await controller.PutExamAssignment(2, update);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("An assignment already exists for this exam and employee combination.", conflictResult.Value);
        }
    }

    [Fact]
    public async Task GetExamAssignment_ReturnsNotFound_WhenAssignmentIsMissing()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();

        using (var context = TestDbContextFactory.CreateContext(connection))
        {
            var controller = new ExamAssignmentsController(context);

            var result = await controller.GetExamAssignment(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
