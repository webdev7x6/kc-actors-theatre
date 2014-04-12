using AutoMapper;

namespace KCActorsTheatre.Services.Mapping
{
    public class AutoMapperService: IMappingService
    {
        public TOutput Map<TInput, TOutput>(TInput source)
        {
            return Mapper.Map<TInput, TOutput>(source);
        }
    }
}