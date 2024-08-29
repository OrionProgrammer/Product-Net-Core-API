using Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Helpers;

namespace Repository { }

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(DataContext context) : base(context) { }

    //fetch all categories by userid.
    public async Task<List<Category>> GetCategoryListByUserId(long userId)
    {
        return await _context!.Category
                         .Join(_context.UserCategory,
                               category => category.CategoryId,
                               userCategory => userCategory.CategoryId,
                               (category, userCategory) => new { category, userCategory })
                         .Where(cu => cu.userCategory.UserId == userId)
                         .Select(cu => cu.category)
                         .AsNoTracking()
                         .AsQueryable()
                         .ToListAsync();
    }

    //select a single category by userId and categoryId. to make sure the category belongs to the user
    public async Task<Category> GetCategoryByUserIdCategoryId(long userId, long categoryId)
    {
        return await _context!.Category
                  .Join(_context.UserCategory,
                        category => category.CategoryId,
                        userCategory => userCategory.CategoryId,
                        (category, userCategory) => new { category, userCategory })
                  .Where(cu => cu.userCategory.UserId == userId && cu.category.CategoryId == categoryId)
                  .Select(cu => cu.category)
                  .FirstOrDefaultAsync();
    }


    //get category list by page number
    public async Task<List<Category>> GetCategoriesByUserIdPaged(long userId, int pageNumber, int pageSize = 10)
    {
        return await _context!.Category
                               .Join(_context.UserCategory,
                                     category => category.CategoryId,
                                     userCategory => userCategory.CategoryId,
                                     (category, userCategory) => new { category, userCategory })
                               .Where(cu => cu.userCategory.UserId == userId)
                               .Select(cu => cu.category)
                               .Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .AsNoTracking()
                               .AsQueryable()
                               .ToListAsync();
    }

    public async Task<int> GetCategoryCountByUserId(long userId)
    {
        return await _context!.Category
                               .Join(_context.UserCategory,
                                     category => category.CategoryId,
                                     userCategory => userCategory.CategoryId,
                                     (category, userCategory) => new { category, userCategory })
                               .Where(cu => cu.userCategory.UserId == userId)
                               .AsNoTracking()
                               .CountAsync();
    }

}
