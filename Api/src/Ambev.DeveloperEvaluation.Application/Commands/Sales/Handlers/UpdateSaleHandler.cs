using Ambev.DeveloperEvaluation.Application.Interface.Sales;
using Ambev.DeveloperEvaluation.Application.Sales.Commands;
using Ambev.DeveloperEvaluation.Application.Services.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Handlers
{
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, Unit>
    {
        private readonly SaleService _saleService;
        private readonly IDiscountService _discountService;

        public UpdateSaleHandler(SaleService saleService, IDiscountService discountService)
        {
            _saleService = saleService;
            _discountService = discountService;
        }

        public async Task<Unit> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = new Sale
            {
                Id = request.Id,
                CustomerId = request.CustomerId,
                BranchId = request.BranchId,
                Items = request.Items.Select(i => new SaleItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await _saleService.UpdateSaleAsync(sale, _discountService);
            return Unit.Value;
        }
    }
}
