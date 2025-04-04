namespace Ambev.DeveloperEvaluation.WebApi.DTOs.Sales;

public class CreateSaleRequest
{
    public Guid CustomerId { get; set; }
    public Guid BranchId { get; set; }
    public List<CreateSaleItemRequest>? Items { get; set; }
}

public class CreateSaleItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
