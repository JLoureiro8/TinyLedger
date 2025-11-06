namespace Domain.Model
{
    public class Transaction
    {
        public Transaction(int customerId, decimal amount, DateTime transactionDate, TransactionType transactionType)
        {
            CustomerId = customerId;
            Amount = amount;
            TransactionDate = transactionDate;
            TransactionType = transactionType;
        }
        public Guid TransactionId { get; private set; }
        public int CustomerId { get; }
        public decimal Amount { get; }
        public DateTime TransactionDate { get; }
        public TransactionType TransactionType { get; }

        public void SetTransactionId(Guid id)
        {
            TransactionId = id;
        }
    }
}
