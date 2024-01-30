using BackEndTest.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Organizer.Context;
using Organizer.Models;

namespace BackEndTest.Repository
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesByUser(string UserId)
        {
            return await _context.Categories
            .Where(t => t != null && (t.UserId == UserId || t.UserId == null))
            .ToListAsync();

        }

        public async Task<IEnumerable<Category>> GetOnlyUserCategories(string UserId)
        {
            return await _context.Categories
            .Where(t => t != null && (t.UserId == UserId))
            .ToListAsync();

        }

        public async Task<Category> GetCategoryById(int id)
        {
            var category = await _context.Categories
                .Where(c => c.CategoryId == id)
                .FirstOrDefaultAsync();

            if (category == null)
                throw new InvalidOperationException($"Bank account not found");

            return category;
        }

        public bool Add(Category category)
        {
            _context.Add(category);
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
