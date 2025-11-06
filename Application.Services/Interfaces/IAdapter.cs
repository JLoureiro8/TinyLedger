namespace Application.Services.Interfaces
{
    public interface IToDtoAdapter<TDomain, TDto>
    {
        TDto ConvertToDto(TDomain domainModel);
    }

    public interface IToDomainAdapter<TDomain, TDto>
    {
        TDomain ConvertToDomainModel(TDto dto);
    }
}
