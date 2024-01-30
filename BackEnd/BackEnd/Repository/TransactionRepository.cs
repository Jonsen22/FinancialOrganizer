using AutoMapper.QueryableExtensions;
using BackEndTest.DTOs;
using BackEndTest.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Organizer.Context;
using Organizer.Models;

namespace BackEndTest.Repository
{
    public class TransactionRepository : BaseRepository, ITransactionRepository
    {
        private readonly AppDbContext _context;
        public TransactionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByUser(string UserId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == UserId)
                .Include(t => t.Category)
                .Include(t => t.BankAccount)
                .ToListAsync();
        }

        public async Task<Transaction> GetTransactionById(int id)
        {
            var transaction = await _context.Transactions
                .Where(t => t.TransactionId == id)
                .FirstOrDefaultAsync();

            if (transaction == null)
                throw new InvalidOperationException($"Bank account not found");

            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByDate(DateTime InitialDate, DateTime FinalDate, string UserId)
        {
            var transaction = await _context.Transactions
              .Where(t => t.Date.Date >= InitialDate.Date && t.Date.Date <= FinalDate.Date && t.UserId == UserId)
              .Include(t => t.Category)
              .Include(t => t.BankAccount)
              .ToListAsync();

            return transaction;
        }

        public bool Add(Transaction transaction)
        {
            _context.Add(transaction);
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
