using Products.Models;
namespace Products.Interfaces
{
  public interface ICommerceInformationRepository
  {
    Task<CommerceInformation> GetCommerceInformation(CancellationToken cancellationToken);
    Task<CommerceInformation> UpdateCommerceInformation(CommerceInformation commerceInformation, CancellationToken cancellationToken);
    Task<bool> SaveCertificate(Stream certificate, CommerceInformation commerceInformation, CancellationToken cancellationToken);
    Task<byte[]?> GetCertificate(string commersName, CancellationToken cancellationToken);
    Task<bool> SaveLogo(Stream Logo, CommerceInformation commerceInformation, CancellationToken cancellationToken);
    Task<byte[]?> GetLogo(string commersName, CancellationToken cancellationToken);
  }
}