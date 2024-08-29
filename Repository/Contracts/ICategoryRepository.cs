using Repository.Helpers;

namespace Repository.Contracts { }

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<List<Category>> GetCategoryListByUserId(long userId);

    Task<Category> GetCategoryByUserIdCategoryId(long userId, long categoryId);

    Task<List<Category>> GetCategoriesByUserIdPaged(long userId, int pageNumber, int pageSize);
    
    Task<int> GetCategoryCountByUserId(long userId);

}
