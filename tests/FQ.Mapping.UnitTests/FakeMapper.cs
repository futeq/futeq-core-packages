namespace FQ.Mapping.UnitTests;

internal sealed class FakeMapper : IObjectMapper
{
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        if (source is null) return default!;
        
        // very dumb “convention” mapper only for tests
        var src = source!;
        if ((typeof(TSource).Name is "Object" || typeof(TSource).Name is "User") && typeof(TDestination).Name is "UserDto")
        {
            dynamic d = src!;
            return (TDestination)(object)new UserDto { Id = d.Id, Name = d.Name };
        }
        if (typeof(TSource).Name is "UserDto" && typeof(TDestination).Name is "User")
        {
            dynamic d = src!;
            return (TDestination)(object)new User { Id = d.Id, Name = d.Name };
        }
        throw new NotSupportedException("Fake mapping not configured");
    }

    public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
        => ProjectionHelpers.ProjectToInMemory<TDestination>(source, this);

    public IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> source)
        => ProjectionHelpers.ProjectToInMemory<TSource, TDestination>(source, this);
}

internal sealed class User { public int Id { get; set; } public string Name { get; set; } = ""; }
internal sealed class UserDto { public int Id { get; set; } public string Name { get; set; } = ""; }