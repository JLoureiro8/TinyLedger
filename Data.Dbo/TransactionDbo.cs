namespace Data.Dbo
{
    public class TransactionDbo
    {
        public TransactionDbo(Guid transactionId, int customerId, decimal amount, DateTime transactionDate, string transactionType)
        {
            TransactionId = transactionId;
            CustomerId = customerId;
            Amount = amount;
            TransactionDate = transactionDate;
            TransactionType = transactionType;
        }

        public Guid TransactionId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
    }
}
