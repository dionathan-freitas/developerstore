﻿using MediatR;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries;

public class GetSaleByIdQuery : IRequest<Sale>
{
    public Guid Id { get; set; }
}
