using System;
using System.ComponentModel.DataAnnotations;

namespace Kallum.Helper
{
    public class ChargeCompletedEvent
    {
        public int Id { get; set; }
        public string? TxRef { get; set; }
        public string? FlwRef { get; set; }
        public string? OrderRef { get; set; }
        public int? PaymentPlan { get; set; }
        public int? PaymentPage { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public decimal ChargedAmount { get; set; }
        public string? Status { get; set; }
        public string? IP { get; set; }
        public string? Currency { get; set; }
        public decimal AppFee { get; set; }
        public decimal MerchantFee { get; set; }
        public int MerchantBearsFee { get; set; }
        public Customer? Customer { get; set; }
        public CardEntity? Entity { get; set; }
        [Display(Name = "event.type")]
        public string? EventType { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string? Phone { get; set; }
        public string? FullName { get; set; }
        public string? CustomerToken { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int AccountId { get; set; }
    }

    public class CardEntity
    {
        public string? Card6 { get; set; }
        public string? CardLast4 { get; set; }
        public string? CardCountryIso { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
