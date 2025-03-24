using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly DefaultContext _context;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        var logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_context, logger);
    }

    [Fact(DisplayName = "Given valid sale data When creating Then creates sale successfully")]
    public async Task Handle_ValidData_CreatesSale()
    {

        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 5,
                    UnitPrice = 100m
                }
            }
        };


        var saleId = await _handler.Handle(command, CancellationToken.None);

        var createdSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == saleId);
        createdSale.Should().NotBeNull();
        createdSale!.Items.Should().ContainSingle();
        createdSale.TotalAmount.Should().Be(450m);
    }
}
