using System.Linq.Expressions;

namespace Launcher.BL.Helpers;

// Helper methods for sorting and filtering database queries.
public static class QueryHelper
{
    // Checks if a search term was provided (not null, not empty, not just spaces).
    // Usage: if (QueryHelper.HasSearchTerm(query.SearchTerm)) { ... }
    public static bool HasSearchTerm(string? searchTerm)
    {
        return string.IsNullOrWhiteSpace(searchTerm) == false;
    }

    // Applies sorting to a query.
    // - query: the database query to sort
    // - keySelector: which property to sort by, e.g. g => g.Name
    // - descending: true = Z-A, false = A-Z
    // Usage: dbQuery = QueryHelper.ApplySort(dbQuery, g => g.Name, query.SortDescending);
    public static IQueryable<T> ApplySort<T, TKey>(IQueryable<T> query, Expression<Func<T, TKey>> keySelector, bool descending)
    {
        if (descending)
        {
            return query.OrderByDescending(keySelector);
        }
        return query.OrderBy(keySelector);
    }
}
