using Ambev.DeveloperEvaluation.Application.Interface.Sales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Services.Sales;

public class SaleService
{
    private readonly DefaultContext _context;
    private readonly ILogger<SaleService> _logger;
    private readonly ISaleEventLogger _eventLogger;

    public SaleService(DefaultContext context, ILogger<SaleService> logger, ISaleEventLogger eventLogger)
    {
        _context = context;
        _logger = logger;
        _eventLogger = eventLogger;
    }

    public async Task<List<Sale>> GetAllSalesAsync()
    {
        return await _context.Sales
            .Include(s => s.Items)
            .ToListAsync();
    }

    public async Task<Sale?> GetSaleByIdAsync(Guid id)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task CreateSaleAsync(Sale sale, IDiscountService discountService)
    {
        discountService.ApplyDiscounts(sale);
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        _eventLogger.Log(sale, SaleEventType.SaleCreated);
    }

    public async Task UpdateSaleAsync(Sale sale, IDiscountService discountService)
    {
        var existingSale = await _context.Sales.Include(s => s.Items).FirstOrDefaultAsync(s => s.Id == sale.Id);
        if (existingSale == null) return;

        existingSale.CustomerId = sale.CustomerId;
        existingSale.BranchId = sale.BranchId;
        existingSale.Items = sale.Items;

        discountService.ApplyDiscounts(existingSale);
        await _context.SaveChangesAsync();

        _eventLogger.Log(existingSale, SaleEventType.SaleModified);
    }

    public async Task CancelItemAsync(Guid saleId, Guid itemId)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == saleId);

        if (sale == null)
            throw new KeyNotFoundException("Venda não encontrada.");

        var item = sale.Items.FirstOrDefault(i => i.Id == itemId);

        if (item == null)
            throw new KeyNotFoundException("Item da venda não encontrado.");

        _context.SaleItems.Remove(item);
        sale.TotalAmount = sale.Items
            .Where(i => i.Id != itemId)
            .Sum(i => i.TotalAmount);

        await _context.SaveChangesAsync();
        _eventLogger.Log(sale, SaleEventType.ItemCancelled, item.Id);
    }

    public async Task DeleteSaleAsync(Guid id)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sale == null)
            throw new KeyNotFoundException("Venda não encontrada.");

        _context.SaleItems.RemoveRange(sale.Items);
        _context.Sales.Remove(sale);

        await _context.SaveChangesAsync();
        _eventLogger.Log(sale, SaleEventType.SaleDeleted);
    }
}
