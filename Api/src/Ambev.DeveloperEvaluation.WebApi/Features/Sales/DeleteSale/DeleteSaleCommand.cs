using MediatR;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale
{
    public class DeleteSaleCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
