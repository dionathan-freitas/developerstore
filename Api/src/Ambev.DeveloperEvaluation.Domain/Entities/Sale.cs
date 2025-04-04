using Ambev.DeveloperEvaluation.Domain.Common;
using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public string SaleNumber { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid BranchId { get; set; }
        public bool IsCancelled { get; set; } = false;
        public List<SaleItem> Items { get; set; } = new();
        public string? CreatedAt { get; set; }
    }
}
