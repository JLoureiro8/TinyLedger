namespace Data.Dbo
{
    public class BalanceDbo
    {
        public Guid Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
