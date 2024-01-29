using Organizer.Models;

namespace BackEndTest.DTOs
{
    public class TransactionDTO
    {
        public int TransactionId {get; set; }
        public  string? Name { get; set; }
        public  float Value { get; set; }
        public  DateTime Date { get; set; }
        public string? Description { get; set; }
        public  char Recurring { get; set; }
        public  string? Type { get; set; } // 0 - income / 1 - outcome
        public BankAccountDTO? BankAccount { get; set; }
        public CategoryDTO? Category { get; set; }
    }
}
