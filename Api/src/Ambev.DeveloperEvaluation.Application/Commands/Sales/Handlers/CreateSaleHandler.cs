using Ambev.DeveloperEvaluation.Application.Interface.Sales;
using Ambev.DeveloperEvaluation.Application.Services.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.Handlers
{
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, Guid>
    {
        private readonly SaleService _saleService;
        private readonly IDiscountService _discountService;

        public CreateSaleHandler(SaleService saleService, IDiscountService discountService)
        {
            _saleService = saleService;
            _discountService = discountService;
        }

        public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                BranchId = request.BranchId,
                Items = request.Items.Select(i => new SaleItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                CreatedAt = DateTime.UtcNow.ToString()
            };

            await _saleService.CreateSaleAsync(sale, _discountService);
            return sale.Id;
        }
    }
}
