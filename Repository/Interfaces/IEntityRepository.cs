using System.Linq.Expressions;

namespace CountryAPI.Repository.Interfaces
{
    public interface IEntityRepository<TEntity> where TEntity : class, new()
    {
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
