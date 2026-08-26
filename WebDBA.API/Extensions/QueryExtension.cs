using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace WebDBA.API.Extensions
{
    public static class QueryExtension
    {
        public static async Task<bool> ExistsAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate)
        {
            return await query.AnyAsync(predicate);
        }
    }
}
