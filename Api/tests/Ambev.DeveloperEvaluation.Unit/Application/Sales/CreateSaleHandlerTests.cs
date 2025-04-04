using Ambev.DeveloperEvaluation.Application.Interface.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Handlers;
using Ambev.DeveloperEvaluation.Application.Services.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly DefaultContext _context;
    private readonly IDiscountService _discountService;
    private readonly ISaleEventLogger _eventLogger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _discountService = Substitute.For<IDiscountService>();
        _eventLogger = Substitute.For<ISaleEventLogger>();

        var logger = Substitute.For<ILogger<SaleService>>();
        var saleService = new SaleService(_context, logger, _eventLogger);

        _handler = new CreateSaleHandler(saleService, _discountService);
    }

    [Fact(DisplayName = "Given valid create sale command When handled Then sale is persisted and ID returned")]
    public async Task Handle_ValidCommand_ReturnsSaleId()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 5,
                    UnitPrice = 10m
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);

        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == result);

        sale.Should().NotBeNull();
        sale!.Items.Should().HaveCount(1);
        sale.CustomerId.Should().Be(command.CustomerId);
        sale.BranchId.Should().Be(command.BranchId);

        _discountService.Received(1).ApplyDiscounts(Arg.Any<Sale>());
        _eventLogger.Received(1).Log(Arg.Any<Sale>(), SaleEventType.SaleCreated);
    }
}
