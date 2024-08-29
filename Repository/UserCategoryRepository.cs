using Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Helpers;

namespace Repository { }

public class UserCategoryRepository : GenericRepository<UserCategory>, IUserCategoryRepository
{
    public UserCategoryRepository(DataContext context) : base(context) { }

    public async Task<bool> CheckIfUserCategoryExists(long categoryId, long userId) => 
        await table!.AnyAsync(uc => uc.UserId == userId && uc.CategoryId == categoryId);
}
