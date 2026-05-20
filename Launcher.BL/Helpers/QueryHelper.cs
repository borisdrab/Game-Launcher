using System.Linq.Expressions;

namespace Launcher.BL.Helpers;

public static class QueryHelper
{
    public static bool HasSearchTerm(string? searchTerm)
    {
        return string.IsNullOrWhiteSpace(searchTerm) == false;
    }

    public static IQueryable<T> ApplySort<T, TKey>(IQueryable<T> query, Expression<Func<T, TKey>> keySelector, bool descending)
    {
        if (descending)
        {
            return query.OrderByDescending(keySelector);
        }
        return query.OrderBy(keySelector);
    }
}
