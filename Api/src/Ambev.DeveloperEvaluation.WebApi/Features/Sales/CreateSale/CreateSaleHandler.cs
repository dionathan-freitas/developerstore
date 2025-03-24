using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using MediatR;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, Guid>
    {
        private readonly DefaultContext _context;

        private readonly ILogger<CreateSaleHandler> _logger;

        public CreateSaleHandler(DefaultContext context, ILogger<CreateSaleHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = new Sale
            {
                CustomerId = request.CustomerId,
                BranchId = request.BranchId,
                Items = new List<SaleItem>()
            };

            foreach (var itemDto in request.Items)
            {
                if (itemDto.Quantity > 20)
                    throw new InvalidOperationException("Não é permitido vender mais de 20 itens de um mesmo produto.");

                decimal discount = 0;
                if (itemDto.Quantity >= 10) discount = 0.20m;
                else if (itemDto.Quantity >= 4) discount = 0.10m;

                var item = new SaleItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    Discount = discount,
                    TotalAmount = itemDto.Quantity * itemDto.UnitPrice * (1 - discount)
                };

                sale.Items.Add(item);
            }

            sale.TotalAmount = sale.Items.Sum(i => i.TotalAmount);

            _context.Sales.Add(sale);
            
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Event: SaleCreated - Sale {SaleId} created at {Time}", sale.Id, DateTime.UtcNow);
            return sale.Id;
        }
    }
}
