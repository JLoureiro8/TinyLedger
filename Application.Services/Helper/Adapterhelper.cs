using Application.Dto;
using Application.Services.Interfaces;
using Domain.Model;

namespace Application.Services.Helper
{
    public class Adapterhelper : IToDomainAdapter<Transaction, TransactionRequestDto>, IToDtoAdapter<Transaction, TransactionResponseDto>, IToDtoAdapter<Balance, BalanceDto>
    {
        public Transaction ConvertToDomainModel(TransactionRequestDto dto)
        {
            return new Transaction(dto.CustomerId, dto.Amount, DateTime.UtcNow, MapStringToTransactionType(dto.TransactionType));
        }

        public TransactionResponseDto ConvertToDto(Transaction domainModel)
        {
            return new TransactionResponseDto
            {
                TransactionId = domainModel.TransactionId,
                CustomerId = domainModel.CustomerId,
                Amount = domainModel.Amount,
                TransactionDate = domainModel.TransactionDate,
                TransactionType = domainModel.TransactionType.ToString()
            };
        }

        public BalanceDto ConvertToDto(Balance domainModel)
        {
            return new BalanceDto
            {
                CustomerId = domainModel.CustomerId,
                Amount = domainModel.Amount,
                ModifiedDate = domainModel.ModifiedDate
            };
        }

        private TransactionType MapStringToTransactionType(string type)
        {
            return type.ToLower() switch
            {
                "deposit" => TransactionType.Deposit,
                "withdrawal" => TransactionType.Withdrawal,
                _ => throw new InvalidOperationException($"Unknown transaction type: {type}")
            };
        }
    }
}
