using Data.Dbo;
using Data.Repository.Interfaces;

namespace Data.Repository.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly List<TransactionDbo> _transactions = new();

        public async Task<TransactionDbo> AddTransactionAsync(TransactionDbo transactionDbo)
        {
            _transactions.Add(transactionDbo);
            return await Task.FromResult(transactionDbo);
        }

        public async Task<IEnumerable<TransactionDbo>> GetTransactionsByCustomerIdAsync(int customerId)
        {
            return await Task.FromResult(_transactions.Where(t => t.CustomerId == customerId).AsEnumerable());
        }
    }
}
