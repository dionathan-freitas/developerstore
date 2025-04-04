namespace Ambev.DeveloperEvaluation.WebApi.DTOs.Sales;

public class UpdateSaleRequest
{
    public Guid CustomerId { get; set; }
    public Guid BranchId { get; set; }
    public List<UpdateSaleItemRequest>? Items { get; set; }
}

public class UpdateSaleItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
