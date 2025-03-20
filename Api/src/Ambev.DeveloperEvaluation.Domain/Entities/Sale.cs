using Ambev.DeveloperEvaluation.Domain.Common;
using System;
using System.Collections.Generic;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public string SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid BranchId { get; set; }
        public List<SaleItem> Items { get; set; } = new();
        public bool IsCancelled { get; set; }
    }
}
