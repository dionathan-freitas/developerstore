using Ambev.DeveloperEvaluation.Application.Services.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using MediatR;

namespace Ambev.DeveloperEvaluation.WebApi.DTOs.Sales
{
    public class DeleteSaleRequest : IRequestHandler<DeleteSaleCommand, Unit>
    {
        private readonly SaleService _saleService;

        public DeleteSaleRequest(SaleService saleService)
        {
            _saleService = saleService;
        }

        public async Task<Unit> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            await _saleService.DeleteSaleAsync(request.Id);
            return await Task.FromResult(Unit.Value);
        }
    }
}
