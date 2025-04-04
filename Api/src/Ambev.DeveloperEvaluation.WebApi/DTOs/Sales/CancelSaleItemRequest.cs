namespace Ambev.DeveloperEvaluation.WebApi.DTOs.Sales;

public class CancelSaleRequest
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
}
