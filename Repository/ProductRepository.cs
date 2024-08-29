using Domain;
using Microsoft.EntityFrameworkCore;
using Repository.Helpers;

namespace Repository { }

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(DataContext context) : base(context) { }

    public string GetLastProductCode() => table.LastOrDefault().Code;

    public async Task<IEnumerable<Product>> GetProductByUserId(long userId)
    {
        return await _context!.Product
                      .Where(pc => pc.CreatedBy == userId)
                      .AsNoTracking()
                      .ToListAsync();
    }

    public async Task<List<Product>> GetProductByCategoryId(long categoryId)
    {
        return await _context!.Product
                         .Join(_context.Category,
                               product => product.CategoryId,
                               category => category.CategoryId,
                               (product, category) => new { product, category})
                         .Where(pc => pc.category.CategoryId == categoryId)
                         .Select(pc => pc.product)
                         .AsNoTracking()
                         .AsQueryable()
                         .ToListAsync();
    }

    //get product list by page number
    public async Task<(int, List<Product>)> GetProductsByUserIdPaged(long userId, int start, int length)
    {
        var totalRecords = await _context.Product.Where(c => c.CreatedBy == userId).CountAsync();
        var products = await _context.Product
                                    .Where(c => c.CreatedBy == userId)
                                    .Skip(start)
                                    .Take(length)
                                    .AsNoTracking()
                                    .ToListAsync();

        return (totalRecords, products);
    }

    public async Task<int> GetProductsCountByUserIdCategoryId(long userId, long categoryId)
    {
        return await _context!.Product
                               .Where(p => p.CreatedBy == userId && p.CategoryId == categoryId)
                               .AsNoTracking()
                               .CountAsync();
    }
}
