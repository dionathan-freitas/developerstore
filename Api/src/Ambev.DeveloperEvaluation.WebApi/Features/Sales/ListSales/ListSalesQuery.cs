using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales
{
    public class ListSalesQuery : IRequest<List<Sale>> { }

    public class ListSalesHandler : IRequestHandler<ListSalesQuery, List<Sale>>
    {
        private readonly DefaultContext _context;

        public ListSalesHandler(DefaultContext context)
        {
            _context = context;
        }

        public async Task<List<Sale>> Handle(ListSalesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .ToListAsync(cancellationToken);
        }
    }
}
