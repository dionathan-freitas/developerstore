using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Sales.Queries;
using Ambev.DeveloperEvaluation.WebApi.DTOs.Sales;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Controllers;

[Route("api/sales")]
[ApiController]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the list of all sales.
    /// </summary>
    /// <returns>List of sales.</returns>
    /// <response code="200">Returns the list of sales.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSales()
    {
        var result = await _mediator.Send(new ListSalesQuery());
        return Ok(result);
    }

    /// <summary>
    /// Retrieves the details of a specific sale by ID.
    /// </summary>
    /// <param name="id">ID of the sale.</param>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSaleById(Guid id)
    {
        var sale = await _mediator.Send(new GetSaleByIdQuery { Id = id });
        return Ok(sale);
    }

    /// <summary>
    /// Creates a new sale.
    /// </summary>
    /// <param name="request">Sale creation data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request)
    {
        var command = new CreateSaleCommand
        {
            CustomerId = request.CustomerId,
            BranchId = request.BranchId,
            Items = request?.Items?.Select(i => new CreateSaleItemCommand
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        var saleId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSaleById), new { id = saleId }, saleId);
    }

    /// <summary>
    /// Updates an existing sale.
    /// </summary>
    /// <param name="id">ID of the sale to update.</param>
    /// <param name="request">Updated sale data.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale(Guid id, [FromBody] UpdateSaleRequest request)
    {
        var command = new UpdateSaleCommand
        {
            Id = id,
            CustomerId = request.CustomerId,
            BranchId = request.BranchId,
            Items = request.Items.Select(i => new UpdateSaleItemCommand
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Deletes a sale.
    /// </summary>
    /// <param name="id">ID of the sale to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale(Guid id)
    {
        await _mediator.Send(new DeleteSaleCommand { Id = id });
        return NoContent();
    }

    /// <summary>
    /// Cancels a specific item within a sale.
    /// </summary>
    /// <param name="saleId">ID of the sale.</param>
    /// <param name="itemId">ID of the item to cancel.</param>
    [HttpDelete("{saleId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> CancelItem(Guid saleId, Guid itemId)
    {
        var command = new CancelSaleItemCommand
        {
            SaleId = saleId,
            ItemId = itemId
        };

        await _mediator.Send(command);
        return NoContent();
    }

}
