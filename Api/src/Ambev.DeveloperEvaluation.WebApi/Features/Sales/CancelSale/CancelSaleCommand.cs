using MediatR;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelItem
{
    public class CancelSaleItemCommand : IRequest<Unit>
    {
        public Guid SaleId { get; set; }
        public Guid ItemId { get; set; }
    }
}
