namespace KCActorsTheatre.Services.Mapping
{
    public interface IMappingService
    {
        TOutput Map<TInput, TOutput>(TInput source);
    }
}
