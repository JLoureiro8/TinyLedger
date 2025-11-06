namespace Application.Dto
{
    public class TransactionRequestDto
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public required string TransactionType { get; set; }
    }
}
