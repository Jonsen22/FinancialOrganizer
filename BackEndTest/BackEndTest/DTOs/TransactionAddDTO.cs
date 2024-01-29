using Organizer.Models;

namespace BackEndTest.DTOs
{
    public class TransactionAddDTO
    {
        public  int BankAccountId { get; set; }
        public  int CategoryId { get; set; }
        public  string? Name { get; set; }
        public  float Value { get; set; }
        public  DateTime Date { get; set; }
        public string? Description { get; set; }
        public  char Recurring { get; set; }
        public  string? Type { get; set; } // 0 - income / 1 - outcome
    }
}
