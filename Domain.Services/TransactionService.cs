using Data.Dbo;
using Data.Repository.Interfaces;
using Domain.Model;
using Domain.Services.Interfaces;

namespace Domain.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IBalanceRepository _balanceRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IToDboAdapter<Transaction, TransactionDbo> _transactionAdapter;
        private readonly IDboToDomainAdapter<Transaction, TransactionDbo> _transactionDomainAdapter;

        public TransactionService(IBalanceRepository balanceRepository, ITransactionRepository transactionRepository, IToDboAdapter<Transaction, TransactionDbo> transactionAdapter, IDboToDomainAdapter<Transaction, TransactionDbo> transactionDomainAdapter)
        {
            _balanceRepository = balanceRepository;
            _transactionRepository = transactionRepository;
            _transactionAdapter = transactionAdapter;
            _transactionDomainAdapter = transactionDomainAdapter;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsHistoryAsync(int customerId)
        {
            var transactionDbos = await _transactionRepository.GetTransactionsByCustomerIdAsync(customerId);
            return transactionDbos.Select(dbo => _transactionDomainAdapter.ConvertToDomainModel(dbo));
        }

        public async Task<Transaction> RecordTransactionAsync(Transaction transaction)
        {
            if (transaction.TransactionType == TransactionType.Withdrawal)
            {
                var currentBalance = await _balanceRepository.GetBalanceAsync(transaction.CustomerId);

                if (currentBalance != null)
                {
                    if (currentBalance.Amount < transaction.Amount)
                    {
                        throw new InvalidOperationException("Insufficient funds");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"No balance available for customer {transaction.CustomerId}. Please make a Deposit");
                }
            }

            var transactionDbo = _transactionAdapter.ConvertToDbo(transaction);
            await _transactionRepository.AddTransactionAsync(transactionDbo);

            // Update balance
            await _balanceRepository.UpdateBalanceAsync(
                transaction.CustomerId,
                transaction.TransactionType == TransactionType.Deposit ? transaction.Amount : -transaction.Amount);

            var response = _transactionDomainAdapter.ConvertToDomainModel(transactionDbo);

            return response;
        }
    }
}
