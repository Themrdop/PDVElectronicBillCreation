using Products.Models;

public interface IClientRepository
{
  Task<Person> AddClient(Person person, CancellationToken cancellationToken);
  Task<IEnumerable<Person>> GetClient(string id, CancellationToken cancellationToken);
}