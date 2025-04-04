using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.Handlers;
using Ambev.DeveloperEvaluation.Application.Services.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
    public class DeleteSaleHandlerTests
    {
        private readonly DefaultContext _context;
        private readonly DeleteSaleHandler _handler;

        public DeleteSaleHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DefaultContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DefaultContext(options);

            var serviceLogger = Substitute.For<ILogger<SaleService>>();
            var handlerLogger = Substitute.For<ILogger<DeleteSaleHandler>>();
            var eventLogger = Substitute.For<ISaleEventLogger>();

            var saleService = new SaleService(_context, serviceLogger, eventLogger);

            _handler = new DeleteSaleHandler(saleService, handlerLogger);
        }

        [Fact(DisplayName = "Given existing sale When deleting Then removes sale successfully")]
        public async Task Handle_ExistingSale_RemovesSale()
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

            var command = new DeleteSaleCommand { Id = sale.Id };

            await _handler.Handle(command, CancellationToken.None);

            var deleted = await _context.Sales.FindAsync(sale.Id);
            deleted.Should().BeNull();
        }

        [Fact(DisplayName = "Given non-existing sale When deleting Then throws KeyNotFoundException")]
        public async Task Handle_NonExistingSale_ThrowsException()
        {
            var command = new DeleteSaleCommand { Id = Guid.NewGuid() };

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Venda não encontrada.");
        }
    }
}
