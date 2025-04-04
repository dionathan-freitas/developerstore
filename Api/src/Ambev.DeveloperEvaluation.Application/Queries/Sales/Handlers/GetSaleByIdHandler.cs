using Ambev.DeveloperEvaluation.Application.Services.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.Handlers;

public class GetSaleByIdHandler : IRequestHandler<GetSaleByIdQuery, Sale>
{
    private readonly SaleService _saleService;
    private readonly ILogger<GetSaleByIdHandler> _logger;

    public GetSaleByIdHandler(SaleService saleService, ILogger<GetSaleByIdHandler> logger)
    {
        _saleService = saleService;
        _logger = logger;
    }

    public async Task<Sale> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleService.GetSaleByIdAsync(request.Id);

        if (sale == null)
        {
            _logger.LogWarning("Venda não encontrada com ID: {SaleId}", request.Id);
            throw new KeyNotFoundException("Venda não encontrada.");
        }

        return sale;
    }
}
