using System.Collections.Immutable;

namespace CampingAI.Domain.Abstractions.Mappers;
public abstract class SimpleMapper<TSource, TDestination> {
    public abstract TDestination Map(TSource src);

    public IEnumerable<TDestination> Map(IEnumerable<TSource> source) {
        var dst = source.Select(m => Map(m)).ToImmutableList();
        return dst;
    }

}