using BackEndTest.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Organizer.Context;
using Organizer.Models;
using System.Security.Claims;

namespace BackEndTest.Repository
{
    public class BankAccountRepository : BaseRepository, IBankAccountRepository
    {
        private readonly AppDbContext _context;

        public BankAccountRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<BankAccount>> GetBankAccountsByUser(string UserId)
        {
            return await _context.BankAccounts.Where(t => t.UserId == UserId).ToListAsync();
        }

        public async Task<BankAccount> GetBankAccountById(int id)
        {
            var bankAccount = await _context.BankAccounts
                .Where(b => b.BankAccountId == id)
                .FirstOrDefaultAsync();

            if (bankAccount == null)
                throw new InvalidOperationException($"Bank account not found");

            return bankAccount;
        }


        public bool Add(BankAccount bankAccount)
        {
            _context.Add(bankAccount);
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
