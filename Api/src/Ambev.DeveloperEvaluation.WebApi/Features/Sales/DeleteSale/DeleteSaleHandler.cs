using Ambev.DeveloperEvaluation.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale
{
    public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, Unit>
    {
        private readonly DefaultContext _context;
        private readonly ILogger<DeleteSaleHandler> _logger;

        public DeleteSaleHandler(DefaultContext context, ILogger<DeleteSaleHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException("Venda não encontrada.");

            _context.SaleItems.RemoveRange(sale.Items);
            _context.Sales.Remove(sale);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Evento simulado: SaleDeleted - Venda {SaleId}", sale.Id);

            return Unit.Value;
        }
    }
}
