using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;

namespace Promasy.Modules.Core.Pagination;

public static class PaginationExtensions
{
    public static async Task<PagedResponse<TObject>> PaginateAsync<TEntity, TObject>(this IQueryable<TEntity> query, PagedRequest request, Expression<Func<TEntity, TObject>> mapper)
    {
        var response = new PagedResponse<TObject>
        {
            Page = request.Page,
            Total = await query.CountAsync()
        };
        if (response.Total < 1)
        {
            return response;
        }

        Expression<Func<TEntity, object>> orderExpression = 
            p => EF.Property<object>(p, !string.IsNullOrEmpty(request.OrderBy) ? request.OrderBy : "Id");
        
        query = request.IsDescending 
            ? query.OrderByDescending(orderExpression) 
            : query.OrderBy(orderExpression);

        response.Collection = await query
            .Skip((request.Page - 1) * request.Offset)
            .Take(request.Offset)
            .Select(mapper)
            .ToListAsync();
        
        return response;
    }
}