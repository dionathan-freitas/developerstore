using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Interface.Sales
{
    public interface IDiscountService
    {
        void ApplyDiscounts(Sale sale);
    }
}