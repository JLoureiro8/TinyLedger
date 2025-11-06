namespace Domain.Model
{
    public class Balance
    {
        public Balance(Guid id, int customerId, decimal amount, DateTime modifiedDate)
        {
            Id = id;
            CustomerId = customerId;
            Amount = amount;
            ModifiedDate = modifiedDate;
        }

        public Guid Id { get; }
        public int CustomerId { get; }
        public decimal Amount { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
