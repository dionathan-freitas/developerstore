using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands;

public class CreateSaleCommand : IRequest<Guid>
{
    public Guid CustomerId { get; set; }
    public Guid BranchId { get; set; }
    public List<CreateSaleItemCommand>? Items { get; set; }
}

public class CreateSaleItemCommand
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
