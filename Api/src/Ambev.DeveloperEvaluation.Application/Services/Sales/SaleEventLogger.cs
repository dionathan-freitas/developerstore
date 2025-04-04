using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.Extensions.Logging;

public class SaleEventLogger : ISaleEventLogger
{
    private readonly ILogger<SaleEventLogger> _logger;

    public SaleEventLogger(ILogger<SaleEventLogger> logger)
    {
        _logger = logger;
    }

    public void Log(Sale sale, SaleEventType eventType, Guid? itemId = null)
    {
        var message = eventType switch
        {
            SaleEventType.SaleCreated => $"[Event:SaleCreated] Sale {sale.Id} created.",
            SaleEventType.SaleModified => $"[Event:SaleModified] Sale {sale.Id} updated.",
            SaleEventType.SaleCancelled => $"[Event:SaleCancelled] Sale {sale.Id} cancelled.",
            SaleEventType.SaleDeleted => $"[Event:SaleDeleted] Sale {sale.Id} deleted.",
            SaleEventType.ItemCancelled => $"[Event:ItemCancelled] Item {itemId} removed from sale {sale.Id}.",
            _ => "[Event:Unknown] An unknown sale event occurred."
        };

        _logger.LogInformation(message);
    }

}
