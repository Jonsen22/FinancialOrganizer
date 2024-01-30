using Organizer.Models;

namespace BackEndTest.Repository.Interfaces
{
    public interface IBankAccountRepository : IBaseRepository
    {
        Task<IEnumerable<BankAccount>> GetBankAccountsByUser(string UserId);
        Task<BankAccount> GetBankAccountById(int id);
        
    }
}
