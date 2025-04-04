using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

public interface ISaleEventLogger
{
    void Log(Sale sale, SaleEventType eventType, Guid? itemId = null);
}
