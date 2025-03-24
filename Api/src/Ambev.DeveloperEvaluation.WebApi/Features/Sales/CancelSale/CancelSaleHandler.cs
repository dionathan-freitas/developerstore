using Ambev.DeveloperEvaluation.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelItem
{
    public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, Unit> 
    {
        private readonly DefaultContext _context;
        private readonly ILogger<CancelSaleItemHandler> _logger;

        public CancelSaleItemHandler(DefaultContext context, ILogger<CancelSaleItemHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException("Venda não encontrada.");

            var item = sale.Items.FirstOrDefault(i => i.Id == request.ItemId);

            if (item == null)
                throw new KeyNotFoundException("Item da venda não encontrado.");

            _context.SaleItems.Remove(item);

            sale.TotalAmount = sale.Items
                .Where(i => i.Id != request.ItemId)
                .Sum(i => i.TotalAmount);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Evento simulado: ItemCancelled - Venda {SaleId}, Item {ItemId}", sale.Id, item.Id);

            return Unit.Value;
        }
    }
}
