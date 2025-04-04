using Ambev.DeveloperEvaluation.Application.Services.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Handlers
{
    public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, Unit>
    {
        private readonly SaleService _saleService;
        private readonly ILogger<DeleteSaleHandler> _logger;

        public DeleteSaleHandler(SaleService saleService, ILogger<DeleteSaleHandler> logger)
        {
            _saleService = saleService;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            await _saleService.DeleteSaleAsync(request.Id);
            _logger.LogInformation("Venda excluída com sucesso, ID: {SaleId}", request.Id);
            return Unit.Value;
        }
    }
}
