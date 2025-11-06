using Data.Dbo;

namespace Data.Repository.Interfaces
{
    public interface IBalanceRepository
    {
        Task<BalanceDbo> GetBalanceAsync(int customerId);
        Task UpdateBalanceAsync(int customerId, decimal amount);
    }
}
