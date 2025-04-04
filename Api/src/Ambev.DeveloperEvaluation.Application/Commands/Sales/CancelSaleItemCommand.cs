using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands;

public class CancelSaleItemCommand : IRequest
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
}
