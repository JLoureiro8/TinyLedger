namespace Domain.Services.Interfaces
{
    using Domain.Model;
    public interface IBalanceService
    {
        Task<Balance> GetCurrentBalanceAsync(int customerId);
    }
}
