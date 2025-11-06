using Data.Repository.Interfaces;
using Domain.Model;
using Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IBalanceRepository _balanceRepository;
        private readonly IDboToDomainAdapter<Balance, Data.Dbo.BalanceDbo> _balanceAdapter;

        public BalanceService(IBalanceRepository balanceRepository, IDboToDomainAdapter<Balance, Data.Dbo.BalanceDbo> balanceAdapter, ILogger<BalanceService> logger)
        {
            _balanceRepository = balanceRepository;
            _balanceAdapter = balanceAdapter;
        }

        public async Task<Balance> GetCurrentBalanceAsync(int customerId)
        {
            var balanceDbo = await _balanceRepository.GetBalanceAsync(customerId);

            if (balanceDbo == null)
            {
                throw new InvalidOperationException($"No balance available for customer {customerId}. Please make a Deposit");
            }

            return _balanceAdapter.ConvertToDomainModel(balanceDbo);
        }
    }
}
