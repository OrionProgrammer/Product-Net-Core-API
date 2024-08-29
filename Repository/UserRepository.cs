using Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Helpers;

namespace Repository { }

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(DataContext context) : base(context) { }

    public User Authenticate(string email, string password)
    {
        return table.Where(u => u.Email == email && u.Password == password).FirstOrDefault();
    }

}
