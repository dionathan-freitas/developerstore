using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
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
            return Ok(await _mediator.Send(new ListSalesQuery()));
        }

        /// <summary>
        /// Retrieves the details of a specific sale by ID.
        /// </summary>
        /// <param name="id">ID of the sale.</param>
        /// <returns>Sale details.</returns>
        /// <response code="200">Returns the sale data.</response>
        /// <response code="404">Sale not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSaleById(Guid id)
        {
            var sale = await _mediator.Send(new GetSaleQuery { Id = id });
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        /// <summary>
        /// Creates a new sale.
        /// </summary>
        /// <param name="command">Sale data.</param>
        /// <returns>Created sale ID.</returns>
        /// <response code="201">Sale successfully created.</response>
        /// <response code="400">Invalid sale data.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleCommand command)
        {
            var saleId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetSaleById), new { id = saleId }, saleId);
        }

        /// <summary>
        /// Updates an existing sale.
        /// </summary>
        /// <param name="id">ID of the sale to update.</param>
        /// <param name="command">Updated sale data.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Sale successfully updated.</response>
        /// <response code="400">Invalid data.</response>
        /// <response code="404">Sale not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSale(Guid id, [FromBody] UpdateSaleCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Deletes a sale.
        /// </summary>
        /// <param name="id">ID of the sale to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Sale successfully deleted.</response>
        /// <response code="404">Sale not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSale(Guid id)
        {
            await _mediator.Send(new DeleteSaleCommand { Id = id });
            return NoContent();
        }

        /// <summary>
        /// Cancels a specific item from a sale.
        /// </summary>
        /// <param name="saleId">ID of the sale.</param>
        /// <param name="itemId">ID of the item to cancel.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Item successfully canceled.</response>
        /// <response code="404">Sale or item not found.</response>
        [HttpDelete("{saleId}/items/{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelItem(Guid saleId, Guid itemId)
        {
            await _mediator.Send(new CancelSaleItemCommand
            {
                SaleId = saleId,
                ItemId = itemId
            });

            return NoContent();
        }
    }

}
