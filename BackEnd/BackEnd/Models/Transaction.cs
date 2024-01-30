using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organizer.Models
{
    public class Transaction
    {
        public required int TransactionId { get; set; }
        [ForeignKey("BankAccountId")]
        public required int BankAccountId { get; set; }
        [ForeignKey("CategoryId")]
        public required int CategoryId { get; set; }   
        public required string? UserId { get; set; }
        public required string? Name { get; set; }
        public required float Value { get; set; }
        public required DateTime Date { get; set; }
        public string? Description { get; set; }
        public required char Recurring { get; set; }
        public required string? Type { get; set; } // 0 - income / 1 - outcome
        public virtual BankAccount? BankAccount { get; set; }
        public virtual Category? Category { get; set; }

    }
}
