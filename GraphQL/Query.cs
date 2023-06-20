using CommanderGQL.Data;
using CommanderGQL.Models;

namespace CommanderGQL.GrapQL;

public class Query
{
    [UseDbContext(typeof(AppDbContext))]
    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10)]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Platform> GetPlatform([ScopedService] AppDbContext context)
    {
        return context.Platforms;
    }

    [UseDbContext(typeof(AppDbContext))]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Command> GetCommand([ScopedService] AppDbContext context)
    {   
        return context.Commands;
    }
}