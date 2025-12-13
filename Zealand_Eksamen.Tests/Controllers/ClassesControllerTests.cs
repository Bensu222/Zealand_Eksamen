using Microsoft.AspNetCore.Mvc;
using Zealand_Eksamen.Controllers;
using Zealand_Eksamen.Models;
using Zealand_Eksamen.Tests.TestUtilities;
using Xunit;

namespace Zealand_Eksamen.Tests.Controllers;

public class ClassesControllerTests
{
    [Fact]
    public async Task GetClass_ReturnsNotFound_WhenClassMissing()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ClassesController(context);

        var result = await controller.GetClass(123);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PostClass_ReturnsCreatedAtAction_WithPersistedEntity()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ClassesController(context);

        var payload = new Class { ClassCode = "CLS-1", Semester = "Fall", Programme = "IT" };

        var result = await controller.PostClass(payload);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(ClassesController.GetClass), created.ActionName);
        var persisted = Assert.IsType<Class>(created.Value);
        Assert.NotEqual(0, persisted.ClassID);
        Assert.Equal("CLS-1", persisted.ClassCode);

        var fromDb = await controller.GetClass(persisted.ClassID);
        var okResult = Assert.IsType<OkObjectResult>(fromDb);
        var fromDbClass = Assert.IsType<Class>(okResult.Value);
        Assert.Equal(persisted.ClassID, fromDbClass.ClassID);
    }

    [Fact]
    public async Task PutClass_ReturnsBadRequest_OnIdMismatch()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ClassesController(context);

        var update = new Class { ClassID = 1, ClassCode = "CLS-1", Semester = "Fall", Programme = "IT" };

        var result = await controller.PutClass(2, update);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteClass_RemovesEntity_WhenFound()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new ClassesController(context);

        var entity = new Class { ClassCode = "CLS-1", Semester = "Fall", Programme = "IT" };
        context.Classes.Add(entity);
        await context.SaveChangesAsync();

        var response = await controller.DeleteClass(entity.ClassID);

        Assert.IsType<NoContentResult>(response);
        Assert.False(context.Classes.Any());
    }
}
