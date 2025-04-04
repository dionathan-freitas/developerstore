using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands;

public class UpdateSaleCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BranchId { get; set; }
    public List<UpdateSaleItemCommand>? Items { get; set; }
}

public class UpdateSaleItemCommand
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
