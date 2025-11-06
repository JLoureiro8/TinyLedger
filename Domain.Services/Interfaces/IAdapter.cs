namespace Domain.Services.Interfaces
{
    public interface IDboToDomainAdapter<TDomain, TDbo>
    {
        TDomain ConvertToDomainModel(TDbo dbo);
    }

    public interface IToDboAdapter<TDomain, TDbo>
    {
        TDbo ConvertToDbo(TDomain domainModel);
    }
}
