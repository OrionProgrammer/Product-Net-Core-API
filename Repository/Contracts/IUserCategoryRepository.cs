using Repository.Helpers;

namespace Repository.Contracts { }

public interface IUserCategoryRepository : IGenericRepository<UserCategory>
{

    Task<bool> CheckIfUserCategoryExists(long categoryId, long userId);
}
