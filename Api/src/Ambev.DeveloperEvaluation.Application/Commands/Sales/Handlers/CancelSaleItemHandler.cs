using Ambev.DeveloperEvaluation.Application.Services.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Handlers;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand>
{
    private readonly SaleService _saleService;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(SaleService saleService, ILogger<CancelSaleItemHandler> logger)
    {
        _saleService = saleService;
        _logger = logger;
    }

    public async Task Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        await _saleService.CancelItemAsync(request.SaleId, request.ItemId);
        _logger.LogInformation("Item {ItemId} da venda {SaleId} cancelado.", request.ItemId, request.SaleId);
    }
}
