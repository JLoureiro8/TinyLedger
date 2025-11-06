using Data.Dbo;

namespace Data.Repository.Interfaces
{
    public interface ITransactionRepository
    {
        Task<TransactionDbo> AddTransactionAsync(TransactionDbo transactionDbo);
        Task<IEnumerable<TransactionDbo>> GetTransactionsByCustomerIdAsync(int customerId);
    }
}
