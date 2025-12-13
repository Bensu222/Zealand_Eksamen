using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Zealand_Eksamen.Controllers;
using Zealand_Eksamen.Models;
using Zealand_Eksamen.Tests.TestUtilities;

namespace Zealand_Eksamen.Tests.Controllers;

public class EmployeesControllerTests
{
    [Fact]
    public async Task GetEmployee_ReturnsNotFound_WhenEmployeeMissing()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new EmployeesController(context);

        var result = await controller.GetEmployee(123);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostEmployee_ReturnsCreatedAtAction_WithPersistedEntity()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new EmployeesController(context);

        var payload = new Employee { FullName = "Jane Doe", Email = "jane@example.com", Role = "Teacher" };

        var result = await controller.PostEmployee(payload);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(EmployeesController.GetEmployee), created.ActionName);
        var persisted = Assert.IsType<Employee>(created.Value);
        Assert.NotEqual(0, persisted.EmployeeID);
        Assert.Equal("Jane Doe", persisted.FullName);

        var fromDb = await controller.GetEmployee(persisted.EmployeeID);
        Assert.Equal(persisted.EmployeeID, fromDb.Value?.EmployeeID);
    }

    [Fact]
    public async Task PutEmployee_ReturnsBadRequest_OnIdMismatch()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new EmployeesController(context);

        var update = new Employee { EmployeeID = 1, FullName = "Jane Doe", Email = "jane@example.com", Role = "Teacher" };

        var result = await controller.PutEmployee(2, update);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteEmployee_RemovesEntity_WhenFound()
    {
        using var connection = TestDbContextFactory.CreateInMemoryDatabase();
        using var context = TestDbContextFactory.CreateContext(connection);
        var controller = new EmployeesController(context);

        var entity = new Employee { FullName = "Jane Doe", Email = "jane@example.com", Role = "Teacher" };
        context.Employees.Add(entity);
        await context.SaveChangesAsync();

        var response = await controller.DeleteEmployee(entity.EmployeeID);

        Assert.IsType<NoContentResult>(response);
        Assert.False(context.Employees.Any());
    }
}
