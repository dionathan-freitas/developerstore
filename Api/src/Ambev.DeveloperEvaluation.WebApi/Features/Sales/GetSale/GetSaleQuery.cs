using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale
{
    public class GetSaleQuery : IRequest<Sale>
    {
        public Guid Id { get; set; }
    }

    public class GetSaleHandler : IRequestHandler<GetSaleQuery, Sale>
    {
        private readonly DefaultContext _context;

        public GetSaleHandler(DefaultContext context)
        {
            _context = context;
        }

        public async Task<Sale> Handle(GetSaleQuery request, CancellationToken cancellationToken)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (sale == null)
                throw new KeyNotFoundException($"Sale with Id {request.Id} not found.");

            return sale;
        }
    }
}
