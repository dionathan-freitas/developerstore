using Ambev.DeveloperEvaluation.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale
{
    public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, Unit>
    {
        private readonly DefaultContext _context;

        public DeleteSaleHandler(DefaultContext context)
        {
            _context = context;
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

            return Unit.Value;
        }
    }
}
