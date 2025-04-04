using Ambev.DeveloperEvaluation.Application.Interface.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Services.Sales
{
    public class DiscountService : IDiscountService
    {
        public void ApplyDiscounts(Sale sale)
        {
            foreach (var item in sale.Items)
            {
                if (item.Quantity > 20)
                {
                    throw new InvalidOperationException($"O item com ID {item.ProductId} ultrapassou o limite de 20 unidades.");
                }
                else if (item.Quantity >= 10)
                {
                    item.Discount = item.UnitPrice * 0.20m * item.Quantity;
                }
                else if (item.Quantity >= 4)
                {
                    item.Discount = item.UnitPrice * 0.10m * item.Quantity;
                }
                else
                {
                    item.Discount = 0;
                }

                item.TotalAmount = item.UnitPrice * item.Quantity - item.Discount;
            }

            sale.TotalAmount = sale.Items.Sum(i => i.TotalAmount);
        }
    }
}
