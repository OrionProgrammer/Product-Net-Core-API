using Repository.Helpers;

namespace Repository.Contracts { }

public interface IUserRepository : IGenericRepository<User>
{
    User Authenticate(string email, string password);
}
