using Data.Dbo;
using Data.Repository.Interfaces;

namespace Data.Repository.Repositories
{
    public class BalanceRepository : IBalanceRepository
    {
        private readonly List<BalanceDbo> _balances = new();

        public Task<BalanceDbo> GetBalanceAsync(int customerId)
        {
            var balance = _balances.FirstOrDefault(b => b.CustomerId == customerId);
            return Task.FromResult(balance);
        }

        public Task UpdateBalanceAsync(int customerId, decimal amount)
        {
            var balance = _balances.FirstOrDefault(b => b.CustomerId == customerId);
            if (balance != null)
            {
                balance.Amount += amount;
                balance.ModifiedDate = DateTime.UtcNow;
            }
            else
            {
                var newBalance = new BalanceDbo
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Amount = amount,
                    ModifiedDate = DateTime.UtcNow,
                };

                _balances.Add(newBalance);
            }

            return Task.CompletedTask;
        }
    }
}