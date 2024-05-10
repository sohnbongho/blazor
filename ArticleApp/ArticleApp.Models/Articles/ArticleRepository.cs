
using Dul.Domain.Common;

namespace ArticleApp.Models
{
    /// <summary>
    /// Repository Class: ADO.net 또는 Dapper 또는 Entity Framework Core
    /// </summary>
    public class ArticleRepository : IArticleRepository
    {
        public Task<Article> AddArticleAsyn(Article article)
        {
            throw new NotImplementedException();
        }

        public Task DeleteArticleAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Article> EditArticleAsync(Article article)
        {
            throw new NotImplementedException();
        }

        public Task<Article> GetAritleByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Article>> GetArticlesAsync()
        {
            throw new NotImplementedException();
        }
        public Task<PagingResult<Article>> GetAllAsync(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
