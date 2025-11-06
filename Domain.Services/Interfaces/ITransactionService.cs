
using Domain.Model;

namespace Domain.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<Transaction>> GetTransactionsHistoryAsync(int customerId);
        Task<Transaction> RecordTransactionAsync(Transaction transaction);
    }
}
