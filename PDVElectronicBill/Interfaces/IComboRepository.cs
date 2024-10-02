using Products.Models;

namespace Products.Interfaces{
    public interface IComboRepository
    {
        Task<bool> DeleteCombo(string id, CancellationToken cancellationToken);
        Task<Combo> GetComboById(string id, CancellationToken cancellationToken);
        Task<IEnumerable<Combo>> GetCombos(CancellationToken cancellationToken);
        Task<Combo> SaveCombo(Combo combo, CancellationToken cancellationToken);
        Task<Combo> UpdateCombo(Combo combo, CancellationToken cancellationToken);
    }
}
