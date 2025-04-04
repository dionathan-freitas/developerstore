using Ambev.DeveloperEvaluation.Application.Services.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.Handlers;

public class ListSalesHandler : IRequestHandler<ListSalesQuery, List<Sale>>
{
    private readonly SaleService _saleService;
    private readonly ILogger<ListSalesHandler> _logger;

    public ListSalesHandler(SaleService saleService, ILogger<ListSalesHandler> logger)
    {
        _saleService = saleService;
        _logger = logger;
    }

    public async Task<List<Sale>> Handle(ListSalesQuery request, CancellationToken cancellationToken)
    {
        var sales = await _saleService.GetAllSalesAsync();

        _logger.LogInformation("Listando {Count} vendas", sales.Count);
        return sales;
    }
}
