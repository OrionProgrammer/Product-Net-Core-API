namespace Repository.Helpers;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IProductRepository ProductRepository { get; }
    IUserCategoryRepository UserCategoryRepository { get; }

    Task Complete();

}
