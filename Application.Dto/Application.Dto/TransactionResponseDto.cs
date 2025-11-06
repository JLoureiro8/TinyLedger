namespace Application.Dto
{
    public class TransactionResponseDto
    {
        public Guid TransactionId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public required string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
