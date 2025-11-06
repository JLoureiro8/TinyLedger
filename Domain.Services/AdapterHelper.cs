using Data.Dbo;
using Domain.Model;
using Domain.Services.Interfaces;

namespace Domain.Services
{
    public class AdapterHelper : IDboToDomainAdapter<Transaction, TransactionDbo>, IToDboAdapter<Transaction, TransactionDbo>, IDboToDomainAdapter<Balance, BalanceDbo>
    {
        public Transaction ConvertToDomainModel(TransactionDbo dto)
        {
            var transaction = new Transaction(dto.CustomerId, dto.Amount, dto.TransactionDate, MapStringToTransactionType(dto.TransactionType));
            transaction.SetTransactionId(dto.TransactionId);
            return transaction;
        }

        public TransactionDbo ConvertToDbo(Transaction domainModel)
        {
            return new TransactionDbo(Guid.NewGuid(), domainModel.CustomerId, domainModel.Amount, domainModel.TransactionDate, domainModel.TransactionType.ToString());
        }

        public Balance ConvertToDomainModel(BalanceDbo dbo)
        {
            return new Balance(dbo.Id, dbo.CustomerId, dbo.Amount, dbo.ModifiedDate);
        }

        private TransactionType MapStringToTransactionType(string type)
        {
            return type.ToLower() switch
            {
                "deposit" => TransactionType.Deposit,
                "withdrawal" => TransactionType.Withdrawal,
                _ => throw new ArgumentException($"Unknown transaction type: {type}")
            };
        }
    }
}
