using Repository.Helpers;

namespace Repository.Contracts { }

public interface IProductRepository : IGenericRepository<Product>
{
    string GetLastProductCode();

    Task<IEnumerable<Product>> GetProductByUserId(long userId);
    Task<List<Product>> GetProductByCategoryId(long categoryId);

    Task<(int, List<Product>)> GetProductsByUserIdPaged(long userId, int start, int length);

    Task<int> GetProductsCountByUserIdCategoryId(long userId, long categoryId);
}
