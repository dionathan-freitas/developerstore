using MediatR;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries;

public class ListSalesQuery : IRequest<List<Sale>>
{
}
