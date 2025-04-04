using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Sales.Handlers;
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

public class UpdateSaleHandlerTests
{
    private readonly DefaultContext _context;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);

        var logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        var discountService = new DiscountService();
        var saleLogger = Substitute.For<ILogger<SaleService>>();
        var eventLogger = Substitute.For<ISaleEventLogger>();

        var saleService = new SaleService(_context, saleLogger, eventLogger);

        _handler = new UpdateSaleHandler(saleService, discountService);
    }

    [Fact(DisplayName = "Given valid sale update When handling Then updates sale successfully")]
    public async Task Handle_ValidUpdate_UpdatesSale()
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 10m,
                    TotalAmount = 20m
                }
            },
            TotalAmount = 20m
        };

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 5,
                    UnitPrice = 10m
                }
            }
        };

        await _handler.Handle(command, CancellationToken.None);

        var updatedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);
        updatedSale.Should().NotBeNull();
        updatedSale!.Items.Should().ContainSingle();
        updatedSale.TotalAmount.Should().Be(45m);
    }

    [Fact(DisplayName = "Given nonexistent sale When updating Then throws KeyNotFoundException")]
    public async Task Handle_NonexistentSale_ThrowsException()
    {
        var command = new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 3,
                    UnitPrice = 10m
                }
            }
        };

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Venda não encontrada.");
    }

    [Fact(DisplayName = "Given item with quantity above 20 When updating Then throws InvalidOperationException")]
    public async Task Handle_QuantityAbove20_ThrowsException()
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 25,
                    UnitPrice = 15m
                }
            }
        };

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Não é permitido vender mais de 20 itens de um mesmo produto.");
    }

    [Fact(DisplayName = "Given 10 identical items When updating Then applies 20% discount")]
    public async Task Handle_Quantity10_Applies20PercentDiscount()
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
            {
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 10,
                    UnitPrice = 100m
                }
            }
        };

        await _handler.Handle(command, CancellationToken.None);

        var updatedSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);
        updatedSale!.TotalAmount.Should().Be(800m);
    }

    [Fact(DisplayName = "When sale is updated Then logs SaleModified event")]
    public async Task Handle_ValidUpdate_LogsSaleModifiedEvent()
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>(),
            TotalAmount = 0
        };
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var eventLogger = Substitute.For<ISaleEventLogger>();
        var saleLogger = Substitute.For<ILogger<SaleService>>();
        var discountService = new DiscountService();
        var saleService = new SaleService(_context, saleLogger, eventLogger);
        var handler = new UpdateSaleHandler(saleService, discountService);

        var command = new UpdateSaleCommand
        {
            Id = sale.Id,
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<UpdateSaleItemCommand>
        {
            new UpdateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                Quantity = 5,
                UnitPrice = 10m
            }
        }
        };

        await handler.Handle(command, CancellationToken.None);

        eventLogger.Received(1).Log(Arg.Is<Sale>(s => s.Id == sale.Id), SaleEventType.SaleModified);
    }

}
