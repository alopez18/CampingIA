using System.Collections.Immutable;

namespace CampingAI.Domain.Abstractions.Mappers;
public abstract class CompleteMapper<TSource, TDestination> : SimpleMapper<TSource, TDestination> {
    public abstract TSource ReverseMap(TDestination src);

    public IEnumerable<TSource> RerverseMap(IEnumerable<TDestination> src) {
        var dst = src.Select(m => ReverseMap(m)).ToImmutableList();
        return dst;
    }

}