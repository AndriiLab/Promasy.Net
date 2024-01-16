namespace Promasy.Modules.Core.Mapper;

public interface IMapper<TSource, TTarget>
{
    TTarget MapFromSource(TSource src);
}

public interface ITwoWayMapper<TSource, TTarget> : IMapper<TSource, TTarget>
{
    TSource MapFromTarget(TTarget tgt);
}

public interface ISyncMapper<TCreateSource, TUpdateSource, TTarget> : IMapper<TCreateSource, TTarget>
{
    void CopyFromSource(TUpdateSource src, TTarget tgt);
}

public interface IQueryableMapper<TSource, TTarget> : IMapper<TSource, TTarget>
{
    IQueryable<TTarget> MapFromQueryableSource(IQueryable<TSource> src);
}