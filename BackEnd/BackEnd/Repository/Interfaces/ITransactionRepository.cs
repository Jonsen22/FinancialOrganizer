using Organizer.Models;

namespace BackEndTest.Repository.Interfaces
{
    public interface ITransactionRepository : IBaseRepository
    {
        Task<IEnumerable<Transaction>> GetTransactionsByUser(string UserId);

        Task<IEnumerable<Transaction>> GetTransactionsByDate(DateTime InitialDate, DateTime FinalDate, string UserId);
        Task<Transaction> GetTransactionById(int id);

    }
}
