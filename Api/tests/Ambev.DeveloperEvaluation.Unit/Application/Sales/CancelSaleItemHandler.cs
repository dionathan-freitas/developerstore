using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelItem;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CancelSaleItemHandlerTests
{
    private readonly DefaultContext _context;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        var logger = Substitute.For<ILogger<CancelSaleItemHandler>>();

        _handler = new CancelSaleItemHandler(_context, logger);
    }

    [Fact(DisplayName = "Given valid item When cancelling Then removes item and updates total")]
    public async Task Handle_ValidRequest_RemovesItem()
    {
        var item = new SaleItem
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 2,
            UnitPrice = 50m,
            TotalAmount = 100m
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem> { item },
            TotalAmount = 100m
        };

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var command = new CancelSaleItemCommand
        {
            SaleId = sale.Id,
            ItemId = item.Id
        };

        await _handler.Handle(command, CancellationToken.None);

        var updatedSale = await _context.Sales.Include(s => s.Items).FirstAsync(s => s.Id == sale.Id);
        updatedSale.Items.Should().BeEmpty();
        updatedSale.TotalAmount.Should().Be(0);
    }

    [Fact(DisplayName = "Given non-existent sale When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_InvalidSale_ThrowsException()
    {
        var command = new CancelSaleItemCommand
        {
            SaleId = Guid.NewGuid(),
            ItemId = Guid.NewGuid()
        };

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Venda não encontrada.");
    }

    [Fact(DisplayName = "Given non-existent item When cancelling item Then throws KeyNotFoundException")]
    public async Task Handle_InvalidItem_ThrowsException()
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid(),
            Items = new List<SaleItem>(),
            TotalAmount = 0m
        };

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var command = new CancelSaleItemCommand
        {
            SaleId = sale.Id,
            ItemId = Guid.NewGuid()
        };

        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Item da venda não encontrado.");
    }
}
