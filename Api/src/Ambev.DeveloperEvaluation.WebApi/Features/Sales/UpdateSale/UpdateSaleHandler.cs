using Ambev.DeveloperEvaluation.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale
{
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, Unit>
    {
        private readonly DefaultContext _context;
        private readonly ILogger<UpdateSaleHandler> _logger;

        public UpdateSaleHandler(DefaultContext context, ILogger<UpdateSaleHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException("Venda não encontrada.");

            _context.SaleItems.RemoveRange(sale.Items);
            sale.Items.Clear();

            sale.CustomerId = request.CustomerId;
            sale.BranchId = request.BranchId;

            foreach (var itemDto in request.Items)
            {
                if (itemDto.Quantity > 20)
                    throw new InvalidOperationException("Não é permitido vender mais de 20 itens de um mesmo produto.");

                decimal discount = 0;
                if (itemDto.Quantity >= 10) discount = 0.20m;
                else if (itemDto.Quantity >= 4) discount = 0.10m;

                var item = new Domain.Entities.SaleItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    Discount = discount,
                    TotalAmount = itemDto.Quantity * itemDto.UnitPrice * (1 - discount)
                };

                sale.Items.Add(item);
            }

            sale.TotalAmount = sale.Items.Sum(i => i.TotalAmount);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Event: SaleModified - Sale {SaleId} modified at {Time}", sale.Id, DateTime.UtcNow);

            return Unit.Value;
        }

    }
}
