using System;
using System.Collections.Generic;
using System.Linq;

public interface ISafeDataContext
{
    IQueryable<T> GetContracts<T>();
}

public class News
{
    public Guid Id { get; set; }
}

public class NewsUserRelations
{
    public Guid UserId { get; set; }
    public Guid NewsId { get; set; }
}

public class TestClass8
{
    private static IQueryable<News> GetNewsWithoutRelationsQuery(ISafeDataContext dataContext, Guid userId)
    {
        var allNews = dataContext.GetContracts<News>();
        var userRelations = dataContext.GetContracts<NewsUserRelations>().Where(r => r.UserId == userId);
        
        // This should NOT trigger CI0010 because of IQueryable
        return allNews.Where(n => !userRelations.Any(r => r.NewsId == n.Id));
    }
}
