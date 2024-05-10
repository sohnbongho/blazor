
using Dul.Domain.Common;

namespace ArticleApp.Models
{
    /// <summary>
    /// Repository 인터페이스
    /// </summary>
    public interface IArticleRepository
    {
        Task<Article> AddArticleAsyn(Article article); // 입력 
        Task<List<Article>> GetArticlesAsync();         // 출력
        Task<Article> GetAritleByIdAsync(int id);       // 상세
        Task<Article> EditArticleAsync(Article article);    // 수정
        Task DeleteArticleAsync(int id);            // 삭제

        Task<PagingResult<Article>> GetAllAsync(int pageIndex, int pageSize); // 페이징 처리
    }
}
