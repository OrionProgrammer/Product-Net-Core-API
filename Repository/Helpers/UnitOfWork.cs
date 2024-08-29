namespace Repository.Helpers;

using System;
using System.Threading.Tasks;
using Domain;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;

    public IUserRepository UserRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IUserCategoryRepository UserCategoryRepository { get; }

    public UnitOfWork(DataContext dataContext,
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IUserCategoryRepository userCategoryRepository)
    {
        this._context = dataContext;
        this.UserRepository = userRepository;
        this.CategoryRepository = categoryRepository;
        this.ProductRepository = productRepository;
        this.UserCategoryRepository = userCategoryRepository;
    }

    public async Task Complete()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context.Dispose();
        }
    }
}