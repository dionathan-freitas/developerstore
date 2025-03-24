using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Ambev.DeveloperEvaluation.ORM;

namespace Ambev.DeveloperEvaluation.Application.Services
{
    public class SaleService
    {
        private readonly DefaultContext _context;

        public SaleService(DefaultContext context)
        {
            _context = context;
        }

        public List<Sale> GetAllSales()
        {
            return _context.Sales.Include(s => s.Items).ToList();
        }

        public Sale GetSaleById(Guid id)
        {
            return _context.Sales.Include(s => s.Items).FirstOrDefault(s => s.Id == id);
        }

        public void CreateSale(Sale sale)
        {
            ApplyDiscounts(sale);
            _context.Sales.Add(sale);
            _context.SaveChanges();
        }

        public void UpdateSale(Sale sale)
        {
            var existingSale = _context.Sales.Include(s => s.Items).FirstOrDefault(s => s.Id == sale.Id);
            if (existingSale == null) return;

            existingSale.CustomerId = sale.CustomerId;
            existingSale.BranchId = sale.BranchId;
            existingSale.Items = sale.Items;

            ApplyDiscounts(existingSale);
            _context.SaveChanges();
        }

        public void CancelSale(Guid id)
        {
            var sale = _context.Sales.Find(id);
            if (sale != null)
            {
                sale.IsCancelled = true;
                _context.SaveChanges();
            }
        }

        private void ApplyDiscounts(Sale sale)
        {
            foreach (var item in sale.Items)
            {
                if (item.Quantity > 20)
                {
                    throw new Exception($"O item com ID {item.ProductId} ultrapassou o limite de 20 unidades.");
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

                item.TotalAmount = (item.UnitPrice * item.Quantity) - item.Discount;
            }

            sale.TotalAmount = sale.Items.Sum(i => i.TotalAmount);
        }
    }
}
