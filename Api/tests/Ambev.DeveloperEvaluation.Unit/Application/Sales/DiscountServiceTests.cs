using Ambev.DeveloperEvaluation.Application.Services.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Services
{
    public class DiscountServiceTests
    {
        private readonly DiscountService _discountService;

        public DiscountServiceTests()
        {
            _discountService = new DiscountService();
        }

        [Fact(DisplayName = "Given item quantity below 4 When applying discount Then no discount is applied")]
        public void ApplyDiscounts_QuantityBelow4_NoDiscount()
        {
            var sale = new Sale
            {
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = Guid.NewGuid(), Quantity = 3, UnitPrice = 100m }
                }
            };

            _discountService.ApplyDiscounts(sale);

            sale.Items[0].Discount.Should().Be(0);
            sale.Items[0].TotalAmount.Should().Be(300m);
        }

        [Fact(DisplayName = "Given item quantity between 4 and 9 When applying discount Then 10 percent discount is applied")]
        public void ApplyDiscounts_QuantityBetween4And9_TenPercent()
        {
            var sale = new Sale
            {
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = Guid.NewGuid(), Quantity = 5, UnitPrice = 100m }
                }
            };

            _discountService.ApplyDiscounts(sale);

            sale.Items[0].Discount.Should().Be(50m);
            sale.Items[0].TotalAmount.Should().Be(450m);
        }

        [Fact(DisplayName = "Given item quantity between 10 and 20 When applying discount Then 20 percent discount is applied")]
        public void ApplyDiscounts_QuantityBetween10And20_TwentyPercent()
        {
            var sale = new Sale
            {
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = Guid.NewGuid(), Quantity = 10, UnitPrice = 100m }
                }
            };

            _discountService.ApplyDiscounts(sale);

            sale.Items[0].Discount.Should().Be(200m);
            sale.Items[0].TotalAmount.Should().Be(800m);
        }

        [Fact(DisplayName = "Given item quantity above 20 When applying discount Then exception is thrown")]
        public void ApplyDiscounts_QuantityAbove20_ThrowsException()
        {
            var productId = Guid.NewGuid();

            var sale = new Sale
            {
                Items = new List<SaleItem>
        {
            new SaleItem { ProductId = productId, Quantity = 25, UnitPrice = 50m }
        }
            };

            var act = () => _discountService.ApplyDiscounts(sale);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"Não é permitido vender mais de 20 itens do produto {productId}.");
        }


        [Fact(DisplayName = "Given multiple items When applying discounts Then total amount is correctly summed")]
        public void ApplyDiscounts_MultipleItems_TotalAmountIsCorrect()
        {
            var sale = new Sale
            {
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = Guid.NewGuid(), Quantity = 5, UnitPrice = 100m },  // 10% -> 450
                    new SaleItem { ProductId = Guid.NewGuid(), Quantity = 10, UnitPrice = 50m },  // 20% -> 400
                    new SaleItem { ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 200m }   // 0%  -> 400
                }
            };

            _discountService.ApplyDiscounts(sale);

            sale.TotalAmount.Should().Be(450m + 400m + 400m);
        }
    }
}
