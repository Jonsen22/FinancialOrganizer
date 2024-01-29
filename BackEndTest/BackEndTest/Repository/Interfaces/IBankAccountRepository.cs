using Organizer.Models;

namespace BackEndTest.Repository.Interfaces
{
    public interface IBankAccountRepository : IBaseRepository
    {
        Task<IEnumerable<BankAccount>> GetBankAccountsByUser(string UserEmail);
        Task<BankAccount> GetBankAccountById(int id);
        
    }
}
