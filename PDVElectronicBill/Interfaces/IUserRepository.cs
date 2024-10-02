using Products.Models;

namespace Products.Interfaces{
    public interface IUserRepository
    {
        Task<bool> DeleteUser(string id, CancellationToken cancellationToken);
        Task<User> GetUserById(string id, CancellationToken cancellationToken);
        Task<IEnumerable<User>> GetUsers(CancellationToken cancellationToken);
        Task<User> SaveUser(User user, CancellationToken cancellationToken);
        Task<User> UpdateUser(User user, CancellationToken cancellationToken);
    }
}