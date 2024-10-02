using Products.Models;

namespace Products.Interfaces
{
    public interface IProductsRepository
    {
        Task<IEnumerable<Product>> GetAllProducts(CancellationToken cancellationToken);
        Task<Product?> ProductById(string? ProductId, CancellationToken cancellationToken);
        Task<Product?> AddProduct(Product product, CancellationToken cancellationToken);
        Task<bool> DeleteProduct(string ProductId, CancellationToken cancellationToken);
        Task<Product> UpdateProduct(Product product, CancellationToken cancellationToken);
        Task<bool> RemoveProductsFromInventary(IEnumerable<Product> products, CancellationToken cancellationToken);
    }
}