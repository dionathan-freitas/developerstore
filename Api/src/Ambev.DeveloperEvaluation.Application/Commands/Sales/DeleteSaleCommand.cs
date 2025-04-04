using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands;

public class DeleteSaleCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}

