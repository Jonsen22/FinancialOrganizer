using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organizer.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        [ForeignKey("BankAccountId")]
        public int BankAccountId { get; set; }
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }   
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public float Value { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public char Recurring { get; set; }
        public string? Type { get; set; } // 0 - income / 1 - outcome
        public virtual BankAccount? BankAccount { get; set; }
        public virtual Category? Category { get; set; }

    }
}
