using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Zealand_Eksamen.Controllers;
using Zealand_Eksamen.Models;
using Zealand_Eksamen.Tests.TestUtilities;

namespace Zealand_Eksamen.Tests.Controllers;

public class ExamsControllerTests
{
    [Fact]
    public async Task GetExam_ReturnsNotFound_WhenMissing()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ExamsController(context);

        var result = await controller.GetExam(987);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostExam_ReturnsCreatedAtAction_WithPersistedEntity()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ExamsController(context);

        var classEntity = new Class { ClassCode = "CLS-1", Semester = "Fall", Programme = "IT" };
        var examType = new ExamType { TypeName = "Written" };
        context.AddRange(classEntity, examType);
        await context.SaveChangesAsync();

        var payload = new Exam
        {
            ExamName = "Networks",
            ClassID = classEntity.ClassID,
            ExamTypeID = examType.ExamTypeID,
            HasSupervision = true
        };

        var result = await controller.PostExam(payload);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ExamsController.GetExam), created.ActionName);
        var persisted = Assert.IsType<Exam>(created.Value);
        Assert.NotEqual(0, persisted.ExamID);
        Assert.Equal("Networks", persisted.ExamName);

        var fromDb = await controller.GetExam(persisted.ExamID);
        Assert.Equal(persisted.ExamID, fromDb.Value?.ExamID);
    }

    [Fact]
    public async Task PutExam_ReturnsBadRequest_OnIdMismatch()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ExamsController(context);

        var update = new Exam { ExamID = 1, ExamName = "Networks", ClassID = 1 };

        var result = await controller.PutExam(2, update);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteExam_RemovesEntity_WhenFound()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ExamsController(context);

        var classEntity = new Class { ClassCode = "CLS-1", Semester = "Fall", Programme = "IT" };
        var examType = new ExamType { TypeName = "Written" };
        context.AddRange(classEntity, examType);
        await context.SaveChangesAsync();

        var exam = new Exam
        {
            ExamName = "Networks",
            ClassID = classEntity.ClassID,
            ExamTypeID = examType.ExamTypeID
        };
        context.Exams.Add(exam);
        await context.SaveChangesAsync();

        var response = await controller.DeleteExam(exam.ExamID);

        Assert.IsType<NoContentResult>(response);
        Assert.False(context.Exams.Any());
    }
}
