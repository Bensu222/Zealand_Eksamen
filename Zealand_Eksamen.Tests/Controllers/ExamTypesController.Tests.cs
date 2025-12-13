using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Zealand_Eksamen.Controllers;
using Zealand_Eksamen.Models;
using Zealand_Eksamen.Tests.TestUtilities;

namespace Zealand_Eksamen.Tests.Controllers;

public class ExamTypesControllerTests
{
    [Fact]
    public async Task GetExamType_ReturnsNotFound_WhenMissing()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ExamTypesController(context);

        var result = await controller.GetExamType(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostExamType_ReturnsCreatedAtAction_WithPersistedEntity()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ExamTypesController(context);

        var payload = new ExamType { TypeName = "Oral" };

        var result = await controller.PostExamType(payload);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ExamTypesController.GetExamType), created.ActionName);
        var persisted = Assert.IsType<ExamType>(created.Value);
        Assert.NotEqual(0, persisted.ExamTypeID);
        Assert.Equal("Oral", persisted.TypeName);

        var fromDb = await controller.GetExamType(persisted.ExamTypeID);
        Assert.Equal(persisted.ExamTypeID, fromDb.Value?.ExamTypeID);
    }

    [Fact]
    public async Task PutExamType_ReturnsBadRequest_OnIdMismatch()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ExamTypesController(context);

        var update = new ExamType { ExamTypeID = 1, TypeName = "Oral" };

        var result = await controller.PutExamType(2, update);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteExamType_RemovesEntity_WhenFound()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ExamTypesController(context);

        var entity = new ExamType { TypeName = "Oral" };
        context.ExamTypes.Add(entity);
        await context.SaveChangesAsync();

        var response = await controller.DeleteExamType(entity.ExamTypeID);

        Assert.IsType<NoContentResult>(response);
        Assert.False(context.ExamTypes.Any());
    }
}
