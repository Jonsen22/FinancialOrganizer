using Organizer.Models;

namespace BackEndTest.Repository.Interfaces
{
    public interface ICategoryRepository : IBaseRepository
    {
        Task<IEnumerable<Category>> GetCategoriesByUser(string UserId);

        Task<IEnumerable<Category>> GetOnlyUserCategories(string UserId);

        Task<Category> GetCategoryById(int id);
    }
}
