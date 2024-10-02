using Products.Models;

namespace Products.Interfaces{
    public interface IElectronicBill
    {
        Task<Bill> SendElectronicBill(Bill bill, byte[] certificate, string ping, CancellationToken cancellationToken);
    }
}